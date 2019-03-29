# Faster
## ����Dapper��ORM��ܣ���С������������׷���Ŀ�ꡣ
## ���䣺237183141@qq.com
## ��лELEVEN��ָ����
## �汾
### V1.0.0.1 ��ɻ�������ɾ�Ĳ顣
### V1.0.0.2 ������ҳ��ѯ���ִ��ͷ���Ϊ�Ժ���дIOC��׼��
### V1.0.0.3 ����DB First��Code First����ģʽ��
####db first
![Image text](https://github.com/JohnnyZhang0628/Faster/blob/master/screen/db_first.png)
####code first
![Image text](https://github.com/JohnnyZhang0628/Faster/blob/master/screen/code_first.png)
### V1.0.0.4 ����IOC����������ע��
## �����ĵ����CURD
``` C#
	
    [TestClass]
    public class UnitTestFaster
    {

        IUserRepository user;
        IDbConnection _dbConnection;
        [TestInitialize]
        public void Init()
        {
           
            // ��ȡ���ݿ�����
            _dbConnection = BaseService._dbConnection;
            //IOC ����
            //1����ȡ����
            Container container = new Container();
            //2��ע������
            container.RegisterType<IUserRepository, UserService>();
            //3������ʵ��
            user = container.Resolve<IUserRepository>();
          
        }

        /// <summary>
        /// ����DB First ��Code First
        /// </summary>
        [TestMethod]
        public void TestMethodDB()
        {
            //Code First
            string modelPath = @"D:\WorkSpace\Faster\Src\Model\bin\Debug\netstandard2.0\Model.dll";
            _dbConnection.CreateTable(modelPath);

            //DB First
            _dbConnection.CreateModels();
        }

        [TestMethod]
        public void TestMethodCURD()
        {
            

            //��������
            List<User> userList = new List<User>();
            for (int i = 0; i < 10000; i++)
            {
                userList.Add(new User
                {
                    UserName = "��ǿ" + (i + 1),
                    Password = "123456",
                    Email = "237183141@qq.com",
                    Phone = "18516328675"
                });
            }
            user.Add(userList);

            //�����޸�
            userList = new List<User>();
            for (int i = 0; i < 100; i++)
            {
                userList.Add(new User
                {
                    UserId = i + 1,
                    UserName = "��ǿ" + (i + 1),
                    Password = "zq",
                    Email = "zq@qq.com",
                    Phone = "zq"
                });
            }
            user.Update(userList);

            //����������ѯ
            var userModel = user.Get<User>(1, "��ǿ1");
            //����������ѯ 
            userList = user.GetList<User>(" where userid>@id", new { id = 10 }).ToList();
            //��ҳ��ѯ
            var result = user.GetPageList<User>("userid ", " where userid>@id", new { id = 10 }, 2, 20);
            // ����������ҳ��
            int count = result.Item1;
            // ��20������40��
            IEnumerable<User> list = result.Item2;

            // ��������ɾ��
            int delRow = user.Remove<User>(1, "��ǿ1");


            //�û��Զ���ӿ�
            user.Login("zq", "123456");

        }

        /// <summary>
        /// ���Դ洢���̲�ѯ
        /// </summary>
        [TestMethod]
        public void TestMethodSP()
        {
            var query = _dbConnection.GetListSP<User>("sp_test");
        }

       

    }
	// IOCע��������
	 public class Container
    {
        private static Dictionary<string, Type> cacheDic = new Dictionary<string, Type>();
        /// <summary>
        /// ע������
        /// </summary>
        /// <typeparam name="IT">������</typeparam>
        /// <typeparam name="T">����ʵ����</typeparam>
        public void RegisterType<IT, T>()
        {
            //���û���
            cacheDic.Add(typeof(IT).FullName, typeof(T));
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <returns></returns>
        public IT Resolve<IT>()
        {
            string key = typeof(IT).FullName;
            Type type = (Type)cacheDic[key];
            object oValue = Create(type);
            return (IT)oValue;
        }

        private object Create(Type type)
        {
            //���ȱ�����ԣ����Ҳ�����������
            var ctorArray = type.GetConstructors();
            ConstructorInfo ctor = null;
            if (ctorArray.Where(c => c.IsDefined(typeof(InjectionConstructorAttribute), true)).Count() > 0)
            {
                ctor = ctorArray.Where(c => c.IsDefined(typeof(InjectionConstructorAttribute), true)).FirstOrDefault();
            }
            else
            {
                ctor = ctorArray.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            }
            var paraArray = ctor.GetParameters();
            if (paraArray.Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            List<object> listPara = new List<object>();
            foreach (var para in paraArray)
            {
                string keyType = para.ParameterType.FullName;
                if (cacheDic.ContainsKey(keyType))
                {
                    object oPara = Create(cacheDic[keyType]);//����ݹ��
                    listPara.Add(oPara);
                }
                else
                    throw new Exception($"please first register {keyType} type");
            }
            return Activator.CreateInstance(type, listPara.ToArray());
        }
    }
	[FasterTable(TableName = "tb_user")] //�Զ�ӳ���ı���
    public class User
    {
		[FasterIdentity] //������ID
        [FasterKey] //��Ϊ����
        public int UserId { get; set; }
        [FasterColumn(ColumnName ="user_name")] //�����еı���
        [FasterKey] //�������
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
	// ������ɾ�Ĳ�ӿ�
	public interface IRepository
    {
        IEnumerable<T> GetList<T>(string strWhere = "",object param=null);
        T Get<T>(params object[] param);

        int Add<T>(IEnumerable<T> modelList);

        int Update<T>(IEnumerable<T> modelList);
        int Remove<T>(params object[] param);
        Tuple<int, IEnumerable<T>> GetPageList<T>(string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10);

    }
	public abstract class BaseService : IRepository
    {
        // ��̬���캯��ʵ�ֵ���
        public static IDbConnection _dbConnection;

        static BaseService()
        {
            _dbConnection = new SqlConnection("server=.;database=test;user id=sa;password=55969126");
        }

        public int Add<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Add<T>(modelList);
        }

        public T Get<T>(params object[] param)
        {
            return _dbConnection.Get<T>(param);
        }

        public IEnumerable<T> GetList<T>(string strWhere = "", object param = null)
        {
            return _dbConnection.GetList<T>(strWhere, param);
        }

        public int Update<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Update<T>(modelList);
        }

        public int Remove<T>(params object[] param)
        {

            return _dbConnection.Remove<T>(param);
        }

        public Tuple<int, IEnumerable<T>> GetPageList<T>(string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10)
        {
            return _dbConnection.GetPageList<T>(order, strWhere, param, pageNum, PageSize);
        }
    }
	// �û��̳нӿںͻ���ʵ����
	public class UserService : BaseService, IUserRepository
    {
        public bool Login(string username, string password)
        {
            return true;
        }
    }

  
```