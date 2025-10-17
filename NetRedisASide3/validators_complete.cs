// Validators/AssuntoValidator.cs
using FluentValidation;
using NetRedisASide3.Models;

namespace NetRedisASide3.Validators;

public class AssuntoValidator : AbstractValidator<Assunto>
{
    public AssuntoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres.")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição não pode exceder 1000 caracteres.");
    }
}

// Validators/MovimentacaoValidator.cs
using FluentValidation;
using NetRedisASide3.Models;

namespace NetRedisASide3.Validators;

public class MovimentacaoValidator : AbstractValidator<Movimentacao>
{
    public MovimentacaoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres.")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição não pode exceder 1000 caracteres.");
    }
}

// Validators/TipoDocumentoValidator.cs
using FluentValidation;
using NetRedisASide3.Models;

namespace NetRedisASide3.Validators;

public class TipoDocumentoValidator : AbstractValidator<TipoDocumento>
{
    public TipoDocumentoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres.")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição não pode exceder 1000 caracteres.");
    }
}