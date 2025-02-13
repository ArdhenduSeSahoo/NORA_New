//using NORA.Model.Base;

//namespace Damco.Common.Specification.Data
//{
//    public class ExcludeDeletedRowsSpecification<T> : Specification<T>
//    {
//        public ExcludeDeletedRowsSpecification()
//        {
//            predicate = x => (x is IDeletedProperties) ? (bool?)x.GetPropValueByName((x as IDeletedProperties).GetPropertyName(z => z.IsDeleted)) != true : true;
//        }
//    }
//}