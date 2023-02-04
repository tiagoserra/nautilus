using SqlKata;
using SqlKata.Compilers;

namespace Infrastructure.Data.Extensions;

public class NoLockCompilerExtensions : SqlServerCompiler
{
    public override string CompileTableExpression(SqlResult ctx, AbstractFrom from)
    {
        return base.CompileTableExpression(ctx, from) + " WITH (NOLOCK)";
    }
}