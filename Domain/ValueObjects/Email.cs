namespace Domain.ValueObjects;

public class Email
{
    public string Endereco { get; private set; }

    public Email(string endereco) => Endereco = endereco;

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var regex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email);
    }
}