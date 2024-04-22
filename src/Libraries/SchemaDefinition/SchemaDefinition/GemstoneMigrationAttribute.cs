
namespace SchemaDefinition
{
    /// <summary>
    /// Mark all migrations with this INSTEAD of [Migration].
    /// </summary>
    public class GemstoneMigrationAttribute : FluentMigrator.MigrationAttribute
    {
        public GemstoneMigrationAttribute(int branchNumber, int year, int month, int day, string author)
           : base(CalculateValue(branchNumber, year, month, day))
        {
            this.Author = author;
        }
        public string Author { get; private set; }
        private static long CalculateValue(int branchNumber, int year, int month, int day)
        {
            return branchNumber * 1000000000000L + year * 100000000L + month * 1000000L + day * 10000L;
        }
    }
}