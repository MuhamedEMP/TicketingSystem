using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TicketingSys.Models
{
    [Tags("ResponseAttachments")]
    public class ResponseAttachment
    {
        public int Id { get; set; }

        public int ResponseId { get; set; }
        [JsonIgnore] // to avoid circular object references
        public Response Response { get; set; }

        public string Path { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }


    }
}
