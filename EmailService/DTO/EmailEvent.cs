using System.Collections.Generic;
namespace EmailService.DTO
{
    public class EmailEvent
    {

        public String EventType { get; set; }

        public List<String> To { get; set; }

        public String Subject { get; set; }

        public Dictionary<String, Object> Data { get; set; }

        //private List<EmailAttachment> attachments;

        public Dictionary<String, Object> Meta { get; set; }
    }

}
