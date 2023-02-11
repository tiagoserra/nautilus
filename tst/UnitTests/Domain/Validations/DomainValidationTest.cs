using Domain.Validations;

namespace UnitTests.Domain.Validations;

public class DomainValidationTest
{
    [Fact]
    public void DomainValidationIsValid()
    {
        var domainValidation = new DomainValidation();
        Assert.True(domainValidation.IsValid());
    }
    
    [Fact]
    public void DomainValidationIsInValid()
    {
        var domainValidation = new DomainValidation();
        domainValidation.When(true, "errorcode", fieldName:"fieldname");
        Assert.False(domainValidation.IsValid());
    }
}