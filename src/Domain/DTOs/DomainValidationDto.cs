namespace Domain.DTOs;

public class DomainValidationDto
{
    public List<Error> Errors { get; private set; } = new();

    public void When(bool condition, string errorCode, string fieldName)
    {
        if(!condition)
            Errors.Add(new Error(errorCode, fieldName));
    }

    public bool IsValid() => Errors.Any();
}

public class Error
{
    public string Code { get; }

    public string Fields { get; }

    public Error(string code, string fields)
    {
        Code = code;
        Fields = fields;
    }
} 
