using Core.Entities;

namespace Core.Contracts;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Base.Core.Contracts;

using Core.QueryResult;

public interface IStatementRepository : IGenericRepository<Statement>
{
}