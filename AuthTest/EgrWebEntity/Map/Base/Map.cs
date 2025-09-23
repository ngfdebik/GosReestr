using FluentNHibernate.Mapping;

namespace EgrWebEntity.Map.Base
{
    public class Map<T> : ClassMap<T> where T : FluentNHibernate.Data.Entity
    {
        public Map(string schema, string tableName)
        {
            Schema(schema); 
            Table(tableName);
            Id(x => x.Id).Column("id").CustomSqlType("Serial").GeneratedBy.Native(schema + "." +  tableName + "_id_seq");
        }
    }
}
