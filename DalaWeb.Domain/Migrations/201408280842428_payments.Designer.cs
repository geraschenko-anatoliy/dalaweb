// <auto-generated />
namespace DalaWeb.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    public sealed partial class payments : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(payments));
        
        string IMigrationMetadata.Id
        {
            get { return "201408280842428_payments"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
