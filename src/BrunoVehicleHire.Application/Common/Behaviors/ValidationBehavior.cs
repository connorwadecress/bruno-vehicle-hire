using FluentValidation;
using MediatR;

namespace BrunoVehicleHire.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> //Open Close Principle : You can add new commands validation without modifying this class
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    //IValidator<TRequest> (singular) resolves exactly one.
    //If a second validator ever got registered for the same command it'd be ignored

    //IEnumerable<IValidator<TRequest>> resolves all of them
    //— zero, one, or many — and ValidationBehavior runs whichever it finds without caring how many that is.

    //^^ this is open closed again. you could add a second validator class for CreateVehicleCommand and ValidationBehaviour wouldnt need to change at all
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle( // works 
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    //you dont touch a file to add new commands validation you just add a new XCommandValidator and register it in the pipeline

    {
        if (!_validators.Any())
        {
            return await next(cancellationToken); //chain of responsibility 
                       // each link in the chain decides whether / or how to pass the request fiurther down
                                         // AKA decorator pattern 
                               // wraps extra behaviour (the validation) around the handler without the handler knowing or caring 
        }


        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(validator =>
                validator.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}