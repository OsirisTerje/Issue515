using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Issue515
{
   
    public class MyController
    {
        [AuthorizeAttribute]
        public void Whatever()
        { }

        public void Nobody()
        { }
    }

    public class AuthorizeAttribute : Attribute
    {

    }

    public class AllowAnonymousAttribute : Attribute
    {

    }



    public class TestIt
    {
    
        public static IEnumerable<TestCaseData> AllControllerMethods
        {
            get
            {
                var masterType = typeof(MyController);
                var types = masterType.Assembly.GetTypes()
                    .Where(x => x.Namespace == masterType.Namespace);

                foreach (var type in types)
                {
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        yield return new TestCaseData(method) { TestName = $"{type.FullName}.{method.Name}" };
                    }
                }
            }
        }



      
        [TestCaseSource(nameof(AllControllerMethods))]
        public void HasAuthorizeAttributes(MethodInfo methodInfo)
        {
            if (methodInfo.Name == nameof(HasAuthorizeAttributes))
                return;
            var authorizeAttribute = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false);
            var allowAnonymousAttribute = methodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);

            if (!authorizeAttribute.Any() && !allowAnonymousAttribute.Any())
                Assert.Fail($"Method '{methodInfo.DeclaringType.FullName}.{methodInfo.Name}' does not have either one of the 'Authorize' or 'AllowAnonymous' attributes");
        }
    }
}
