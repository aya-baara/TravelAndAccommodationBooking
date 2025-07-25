﻿namespace BookingPlatform.Core.Models;

public class EmailMessage
{
    public string To { get; set; }
    public string Subject { get; set; } 
    public string Body { get; set; }
    public bool IsHtml { get; set; }
    public string? From { get; set; }
    public List<EmailAttachment>? Attachments { get; set; }
}

