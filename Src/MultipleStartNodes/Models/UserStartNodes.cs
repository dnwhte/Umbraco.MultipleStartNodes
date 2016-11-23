using System.Runtime.Serialization;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace MultipleStartNodes.Models
{
    [TableName("userStartNodes")]
    [PrimaryKey("userId", autoIncrement = false)]
    [ExplicitColumns]
    [DataContract(Name = "userStartNodes")]
    public class UserStartNodes
    {
        [Column("userId")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [DataMember(Name = "userId")]
        public int UserId { get; set; }

        [Column("content")]
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [Column("media")]
        [DataMember(Name = "media")]
        public string Media { get; set; }
    }
}
