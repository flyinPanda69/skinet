using System.Linq;
using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace InfraStructure.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
        {
            var query = inputQuery;
            if(spec.Criteria != null){
                query = query.Where(spec.Criteria);
            }
            if(spec.OrderBy != null){
                query = query.OrderBy(spec.OrderBy);
            }
            if(spec.OrderByDescending != null){
                query = query.OrderByDescending(spec.OrderByDescending);
            }
            if(spec.isPagingEnabled){
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            //Here order matters, we want the paging to happen after any filtering or sorting.
            //If we filtering our results early then we wouldnt want to page our results before what knowing what results are we paging.
            
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
        
    }
}