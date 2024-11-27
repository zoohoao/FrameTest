using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YamlDotNet.Serialization;

namespace YamlDotNet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var person = new Person
            {
                Name = "John Doe",
                Age = 30,
                IsEmployed = true
            };
            // 创建序列化器
            var serializer = new SerializerBuilder().Build();

            // 序列化对象为 YAML 字符串
            var yaml = serializer.Serialize(person);
            Console.WriteLine(yaml);
            using (var writer = new StreamWriter("person.yaml"))
            {
                serializer.Serialize(writer, person);
            }
        }

        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsEmployed { get; set; }
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            var deserializer = new DeserializerBuilder().Build();
            using (var reader = new StreamReader("person.yaml"))
            {
                var person = deserializer.Deserialize<Person>(reader);
                Console.WriteLine($"Name: {person.Name}, Age: {person.Age}, Employed: {person.IsEmployed}");
            }
        }
    }
}