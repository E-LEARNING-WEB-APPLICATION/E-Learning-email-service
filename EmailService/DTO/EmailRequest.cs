namespace EmailService.DTO
{
    public class EmailRequest
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public Dictionary<string, string> data { get; set; }
    }
}
