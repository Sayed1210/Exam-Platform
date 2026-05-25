namespace Exam.Service.EmailTemplates;

using System.Net;

public static class EmailTemplateBuilder
{
    public static string BuildExamInvitation(string invitationLink, string startDeadline, string endDeadline)
    {
        return Build(new EmailTemplateContent(
            Header: "Examination Invitation",
            Greeting: "Hello,",
            MessageHtml: "You have been invited to complete an assessment on the <b>Enozom</b> platform.",
            NoticeTitle: $"Time Window: {WebUtility.HtmlEncode(startDeadline)} - {WebUtility.HtmlEncode(endDeadline)}",
            NoticeText: "Please ensure you start the exam before this date, as your link will expire.",
            ActionUrl: invitationLink,
            ActionText: "Begin Examination"));
    }

    public static string BuildPasswordReset(string resetLink, string firstName)
    {
        var greetingName = string.IsNullOrWhiteSpace(firstName)
            ? "Hello,"
            : $"Hello {WebUtility.HtmlEncode(firstName)},";

        return Build(new EmailTemplateContent(
            Header: "Reset Your Password",
            Greeting: greetingName,
            MessageHtml: "Use the link below to reset your password.",
            NoticeTitle: "LINK EXPIRES: 1 hour",
            NoticeText: "If you did not request a password reset, you can ignore this email.",
            ActionUrl: resetLink,
            ActionText: "Reset Password"));
    }

    private static string Build(EmailTemplateContent content)
    {
        var actionUrl = WebUtility.HtmlEncode(content.ActionUrl);

        return $@"
            <div style='background-color: #f4f4f7; padding: 30px; font-family: sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; border: 1px solid #e1e1e1;'>
                    <div style='background-color: #2c3e50; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 22px;'>{WebUtility.HtmlEncode(content.Header)}</h1>
                    </div>
                    <div style='padding: 30px;'>
                        <p style='font-size: 16px; color: #333;'>{content.Greeting}</p>
                        <p style='font-size: 16px; color: #555; line-height: 1.5;'>
                            {content.MessageHtml}
                        </p>
                        <div style='background-color: #fff5f5; border-left: 4px solid #c0392b; padding: 15px; margin: 20px 0;'>
                            <p style='margin: 0; color: #c0392b; font-weight: bold; font-size: 14px;'>
                                {content.NoticeTitle}
                            </p>
                            <p style='margin: 5px 0 0 0; color: #7f8c8d; font-size: 13px;'>
                                {content.NoticeText}
                            </p>
                        </div>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{actionUrl}' style='background-color: #2c3e50; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                                {WebUtility.HtmlEncode(content.ActionText)}
                            </a>
                        </div>
                    </div>
                    <div style='padding: 20px; background-color: #f9f9f9; text-align: center; font-size: 12px; color: #999;'>
                        This is an automated message. Please do not reply.
                    </div>
                </div>
            </div>";
    }

    private sealed record EmailTemplateContent(
        string Header,
        string Greeting,
        string MessageHtml,
        string NoticeTitle,
        string NoticeText,
        string ActionUrl,
        string ActionText);
}
