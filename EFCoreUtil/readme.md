EFCoreUtil  
# unitofwork  
startup ���ע��
```
   services.AddDbContext<IRDbContext>().AddUnitOfWork<IRDbContext>();
```
ʹ�÷���
```
public ValuesController(IUnitOfWork unitOfWork)
{
    _unitOfWork = unitOfWork;

    var userRepo = unitOfWork.GetRepository<User>();
    var postRepo = unitOfWork.GetRepository<Post>();
    var user = new User();
    userRepo.Insert(user);
    unitOfWork.SaveChanges();
    var find = userRepo.Find(user.UserId);
    find.Password = "p@ssword";

    unitOfWork.SaveChanges();
}
```
ע�⣺ InsertAsync �ȷ��� This method is async only to allow special value generators, such as the one
 used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
to access the database asynchronously. For all other cases the non async method
 should be used. һ��������Ԫ �ύʱ����ȥsqlִ�У�ʹ���첽  
# SQLִ��  
ʹ��QuerySqlAsync/QuerySqlDataReaderAsync/ExecuteScalarAsyncִ��sql���ڲ�ʹ��Dapperִ��sql���Զ���¼sql��䵽��־��DEBUG����
```
 var groups = await _unitwork.QuerySqlAsync<ResultHistoryGroupByDate>(@"
SELECT
  to_char(query_datetime, 'YYYY-MM-DD') AS CreationDate,
  count(1)                              AS Total
FROM ir.query_history
WHERE is_deleted = FALSE
GROUP BY to_char(query_datetime, 'YYYY-MM-DD')
ORDER BY CreationDate
LIMIT @pagesize
OFFSET @start", new { start = 0, pagesize = 20 });
```
