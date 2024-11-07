using Gemstone.Diagnostics;
using Gemstone.IO;
using Gemstone.Reflection.MemberInfoExtensions;
using Gemstone.StringExtensions;
using Gemstone.Timeseries.Adapters;
using Gemstone.TypeExtensions;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Adapters;
using openHistorian.Data.Types;
using Org.BouncyCastle.Crypto.Parameters;
using ServiceInterface;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace openHistorian.WebUI.Controllers;

public static class AdapterCollectionHelper<T> 
{
    public class AdapterTypeDescription
    {
        public string Assembly { get; set; } = String.Empty;
        public string AssemblyLocation { get; set; } = String.Empty;
        public string TypeName { get; set; } = String.Empty;
        public string Header { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public Type? Type { get; set; } = null;
    }

    private static Type adapterType => typeof(T);

    /// <summary>
    /// Returns all adapters of type <see cref="{T}"/> in the application directory.
    /// </summary>
    /// <returns> A <see cref="IEnumerable{AdapterTypeDescription}"/> describing all available adapters of type <see cref="{T}"/></returns>
    public static IEnumerable<AdapterTypeDescription> GetAdapters()
    {
        return adapterType
                   .LoadImplementations(FilePath.GetAbsolutePath("").EnsureEnd(Path.DirectorySeparatorChar))
                   .Distinct()
                   .Where(type => GetEditorBrowsableState(type) != EditorBrowsableState.Never)
                   .Select(GetDescription);
    }

    /// <summary>
    /// Gets the editor browsable state of the given type. This method will
    /// search for a <see cref="EditorBrowsableAttribute"/> using reflection.
    /// If none is found, it will default to <see cref="EditorBrowsableState.Always"/>.
    /// </summary>
    /// <param name="type">The type for which an editor browsable state is found.</param>
    /// <param name="adapterType">The type to be searched for in the assemblies.</param>
    /// <returns>
    /// Either the editor browsable state as defined by an <see cref="EditorBrowsableAttribute"/>
    /// or else <see cref="EditorBrowsableState.Always"/>.
    /// </returns>
    private static EditorBrowsableState GetEditorBrowsableState(Type type) =>
        type.TryGetAttribute(out EditorBrowsableAttribute? editorBrowsableAttribute) ?
            editorBrowsableAttribute.State : EditorBrowsableState.Always;

    /// <summary>
    /// Gets a description of the given type. This method will search for a
    /// <see cref="DescriptionAttribute"/> using reflection. If none is found,
    /// it will default to the <see cref="Type.FullName"/> of the type.
    /// </summary>
    /// <param name="type">The type for which a description is found.</param>
    /// <returns>
    /// Either the description as defined by a <see cref="DescriptionAttribute"/>
    /// or else the <see cref="Type.FullName"/> of the parameter.
    /// </returns>
    private static AdapterTypeDescription GetDescription(Type type)
    {
        
        AdapterTypeDescription adapterTypeDescription = new AdapterTypeDescription()
        {
            Assembly = AssemblyName.GetAssemblyName(type.Assembly.Location).Name ?? String.Empty,
            AssemblyLocation = type.Assembly.Location,
            TypeName = type.FullName ?? String.Empty,
            Header = type.Name,
            Type = type
        };

        string[] splitDescription = type.TryGetAttribute(out DescriptionAttribute descriptionAttribute) ?
            descriptionAttribute?.Description.ToNonNullNorEmptyString(type.FullName).Split(':') :
            new[] { type.FullName ?? string.Empty };

        if (splitDescription.Length > 1)
        {
            adapterTypeDescription.Header = splitDescription[0].Trim();
            adapterTypeDescription.Description = splitDescription[1].Trim();
        }
        else
        {
            adapterTypeDescription.Description = splitDescription[0].Trim();
        }

        return adapterTypeDescription;
    }

    private static Type? GetType(string assemblyName, string typeName) 
    {
        return GetAdapters()
            .Where((a) => a.Assembly.Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
            .SingleOrDefault(a => a.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase))?.Type;


    }
    
    public static IEnumerable<ConnectionParameter> GetConnectionParameters(string assemblyName, string typeName, string connectionString) => 
        GetConnectionParameters(GetType(assemblyName, typeName), connectionString);
    
    public static IEnumerable<ConnectionParameter> GetConnectionParameters(Type? type, string connectionString) 
    {
        if ( type is null)
        {
            return new List<ConnectionParameter>();
        }
     
        // Get the list of properties with ConnectionStringParameterAttribute annotations.
        IEnumerable<PropertyInfo> infoList = adapterType.GetProperties()
            .Where(info => info.TryGetAttribute(typeof(ConnectionStringParameterAttribute)?.FullName ?? string.Empty, out Attribute? _));

        return infoList.Select(info => ConnectionParameter.GetConnectionParameter(info, connectionString));
    }

    public static Dictionary<string,Func<ControllerBase, IActionResult>> GetUIResources(string typeName, string assemblyName)
    {
        return GetType(assemblyName,typeName).GetMethods()
            .Where(method => method.TryGetAttribute(out UserInterfaceResourceAttribute? attribute))
            .ToDictionary(method => method.GetCustomAttribute<UserInterfaceResourceAttribute>()!.ResourceIdentifier,
                method => (Func<ControllerBase, IActionResult>)((controller) => method.Invoke(null,new object[] { controller }) as IActionResult ?? controller.NotFound()));
    }
}
