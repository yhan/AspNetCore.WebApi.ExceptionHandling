namespace Test
{
    using System;
    using System.Runtime.Remoting.Contexts;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using NUnit.Framework;

    [TestFixture]
    public class Class1
    {
        [Test]
        public void Should_when_Condition()
        {
            var counter = 0;
            try
            {
                throw new Exception("dqsfqsdf");
            }
            finally
            {
                counter++;
            }

            TestContext.WriteLine($"Counter = {counter}");
        }

        [Test]
        public void Serialize_datetimeoffset()
        {
            var myCommand = new MyCommand(10, DateTimeOffset.UtcNow);
            var serializeObject = JsonConvert.SerializeObject(myCommand, new JsonSerializerSettings() { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() });

            TestContext.WriteLine(serializeObject);
        }
    }


    public class MyCommand : Command
    {
        [JsonConstructor]
        public MyCommand(int id, DateTimeOffset removeFrom) : base(removeFrom)
        {
            this.Id = id;
        }
        
        public int Id { get; }
    }

    public abstract class Command
    {
        public DateTimeOffset RemoveFrom { get; }

        public Command(DateTimeOffset removeFrom)
        {
            this.RemoveFrom = removeFrom;
        }
    }
}