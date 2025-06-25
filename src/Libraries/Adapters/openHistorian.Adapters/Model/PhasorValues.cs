// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using Gemstone.Timeseries;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model;

public class PhasorValues
{

    private string m_magnitudeId;
    private MeasurementKey m_magnitudeKey = MeasurementKey.Undefined;
    private string m_angleId;
    private MeasurementKey m_angleKey = MeasurementKey.Undefined;

    public string Device
    {
        get;
        set;
    }

    public int? DeviceID
    {
        get;
        set;
    }

    [Label("Magnitude Tag Name")]
    [Required]
    [StringLength(200)]
    public string MagnitudePointTag
    {
        get;
        set;
    }
    [Label("Angle Tag Name")]
    [Required]
    [StringLength(200)]
    public string AnglePointTag
    {
        get;
        set;
    }

    public string MagnitudeID
    {
        get => m_magnitudeId;
        set
        {
            m_magnitudeId = value;

            if (!MeasurementKey.TryParse(m_magnitudeId, out m_magnitudeKey))
                m_magnitudeKey = MeasurementKey.Undefined;
        }
    }

    public string AngleID
    {
        get => m_angleId;
        set
        {
            m_angleId = value;

            if (!MeasurementKey.TryParse(m_angleId, out m_angleKey))
                m_angleKey = MeasurementKey.Undefined;
        }
    }

    [Label("Phasor ID")]
    public int? PhasorID
    {
        get;
        set;
    }

    [PrimaryKey(true)]
    [Label("Unique Angle Signal ID")]
    public Guid? AngleSignalID
    {
        get;
        set;
    }

    [PrimaryKey(true)]
    [Label("Unique Magnitude Signal ID")]
    public Guid? MagnitudeSignalID
    {
        get;
        set;
    }

    [Label("Angle Signal Reference")]
    [Required]
    [StringLength(200)]
    public string AngleSignalReference
    {
        get;
        set;
    }

    [Label("Magnitude Signal Reference")]
    [Required]
    [StringLength(200)]
    public string MagnitudeSignalReference
    {
        get;
        set;
    }

    [Label("Phasor Type")]
    public char? Type
    {
        get;
        set;
    }
    public int SourceIndex
    {
        get;
        set;
    }

    public char? Phase
    {
        get;
        set;
    }

    public string Company
    {
        get;
        set;
    }

    public decimal Longitude
    {
        get;
        set;
    }

    public decimal Latitude
    {
        get;
        set;
    }

    public DateTime UpdatedOn
    {
        get;
        set;
    }

    public string ID
    {
        get => m_magnitudeId;
        set
        {
            m_magnitudeId = value;

            if (!MeasurementKey.TryParse(m_magnitudeId, out m_magnitudeKey))
                m_magnitudeKey = MeasurementKey.Undefined;
        }
    }

    [PrimaryKey(true)]
    [Label("Unique Signal ID")]
    public Guid? SignalID
    {
        get;
        set;
    }

    [Label("Alternate Tag Name")]
    public string AlternateTag
    {
        get;
        set;
    }
}