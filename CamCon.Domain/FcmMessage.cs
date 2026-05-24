namespace CamCon.Domain;

public class FcmMessage
{
    public MessageContent? Message { get; set; }
}

public class MessageContent
{
    public string Token { get; set; } = "your Token";
    public NotificationContent? Notification { get; set; }
}

public class NotificationContent
{
    public string Title { get; set; } = "Hello!";
    public string Body { get; set; } = "Greetings From Developer";
    public string Image { get; set; } = "Image URL";
}