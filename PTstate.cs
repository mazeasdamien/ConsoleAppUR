using Rti.Dds.Subscription;
using Rti.Types.Dynamic;


namespace ConsoleAppUR
{
    public class PTstate: Program
    {
        public static void RunSubscriber()
        {
            var typeFactory = DynamicTypeFactory.Instance;

            var PTprocessed = typeFactory.BuildStruct()
               .WithName("PTstate")
               .AddMember(new StructMember("value", typeFactory.GetPrimitiveType<bool>()))
               .Create();

            DataReader<DynamicData> reader = SetupDataReader("PTstate_Topic", subscriber, PTprocessed);
            while (true)
            {
                using var samples = reader.Take();
                foreach (var sample in samples)
                {
                    if (sample.Info.ValidData)
                    {
                        DynamicData data = sample.Data;

                        if (data.GetValue<bool>("value") == true)
                        {
                            pt_processed = true;
                        }
                        else
                        {
                            pt_processed = false;
                        }
                    }
                }
            }
        }
    }
}
