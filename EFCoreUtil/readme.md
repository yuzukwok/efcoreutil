EFCoreUtil  
# unitofwork  
startup 添加注入
```
   services.AddDbContext<IRDbContext>().AddUnitOfWork<IRDbContext>();
```
使用方法
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
注意： InsertAsync 等方法 This method is async only to allow special value generators, such as the one
 used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
to access the database asynchronously. For all other cases the non async method
 should be used. 一个工作单元 提交时（送去sql执行）使用异步  
# SQL执行  
使用QuerySqlAsync/QuerySqlDataReaderAsync/ExecuteScalarAsync执行sql，内部使用Dapper执行sql，自动记录sql语句到日志（DEBUG级别）
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
