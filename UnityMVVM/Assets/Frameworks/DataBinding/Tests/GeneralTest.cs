using NUnit.Framework;
using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.DataBinding.Tests
{
    public class GeneralTest
    {
        public class BasicNestedTestModel : ObservableObject
        {
            private bool isAlive;
            public bool IsAlive
            {
                get => isAlive;
                set
                {
                    isAlive = value;
                    OnPropertyChanged();
                }
            }

            private int age;
            public int Age
            {
                get => age;
                set
                {
                    age = value;
                    OnPropertyChanged();
                }
            }
        }

        public class BasicTestModel : ObservableObject
        {
            private string title;
            public string Title
            {
                get => title;
                set
                {
                    title = value;
                    OnPropertyChanged();
                }
            }

            private BasicNestedTestModel nested;
            public BasicNestedTestModel Nested
            {
                get => nested;
                set
                {
                    nested = value;
                    OnPropertyChanged();
                }
            }
        }

        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void Teardown()
        {

        }

        [Test]
        public void OneWayBinding()
        {
            (DataContext Context, TestBinder Binder) binding = CreateBindingStructure();

            binding.Binder.SetPath("Title");
            binding.Binder.SetDataContext(binding.Context);
            binding.Binder.FormBindingConnection();

            // Load model
            BasicTestModel model = new();
            binding.Context.Data = model;

            // Set data
            model.Title = "Correct value";

            Assert.AreEqual("Correct value", binding.Binder.Value);

            DestroyBindingStructure(binding);
        }

        [Test]
        public void OneWayNestedBinding()
        {
            (DataContext Context, TestBinder Binder) binding = CreateBindingStructure();

            binding.Binder.SetPath("Nested.Age");
            binding.Binder.SetDataContext(binding.Context);
            binding.Binder.FormBindingConnection();

            // Load model
            BasicTestModel model = new()
            {
                Nested = new BasicNestedTestModel()
                {
                    Age = 64
                }
            };
            binding.Context.Data = model;

            // Set data
            model.Nested.Age = 64;

            Assert.AreEqual(64, binding.Binder.Value);

            DestroyBindingStructure(binding);
        }

        [Test]
        public void NewInstance()
        {
            (DataContext Context, TestBinder Binder) binding = CreateBindingStructure();

            binding.Binder.SetPath("Nested.Age");
            binding.Binder.SetDataContext(binding.Context);
            binding.Binder.FormBindingConnection();

            // Load model
            BasicTestModel model = new()
            {
                Nested = new BasicNestedTestModel()
                {
                    Age = 64
                }
            };
            binding.Context.Data = model;

            // Set data
            model.Nested.Age = 64;

            Assert.AreEqual(64, binding.Binder.Value);

            // Re-assign intance
            model.Nested = new BasicNestedTestModel() { Age = 18 };

            Assert.AreEqual(18, binding.Binder.Value);

            DestroyBindingStructure(binding);
        }

        [Test]
        public void SubDataContext()
        {
            // TODO: do nested int value
        }

        [Test]
        public void ValueConverter()
        {
            // TODO

            /*IList<IValueConverter> converters = new List<IValueConverter>() { new NegativeSignConverter() };
            (DataContext context, TestBinder binder) binding = CreateBindingStructure();

            binding.binder.SetPath("Number");
            binding.binder.SetDataContext(binding.context);
            binding.binder.SetConverters(converters);
            binding.binder.FormBindingConnection();

            yield return null;

            // Load data
            var loadDataTask = Task.Run(async () =>
            {
                JsonNode node = Json.Parse("{ 'Number': '50' }");
                await binding.context.LoadData(node);
            });

            while (!loadDataTask.IsCompleted)
                yield return null;

            Assert.AreEqual("-50", binding.binder.Value);

            DestroyBindingStructure(binding);*/
        }

        [Test]
        public void StringFormat()
        {
            (DataContext Context, TestBinder Binder) binding = CreateBindingStructure();

            binding.Binder.SetPath("Title");
            binding.Binder.SetDataContext(binding.Context);
            binding.Binder.SetStringFormat("Level {0}");
            binding.Binder.FormBindingConnection();

            // Load model
            BasicTestModel model = new();
            binding.Context.Data = model;

            // Set data
            model.Title = "5";

            Assert.AreEqual("Level 5", binding.Binder.Value);

            DestroyBindingStructure(binding);
        }

        private (DataContext context, TestBinder binder) CreateBindingStructure()
        {
            GameObject container = new("container");
            GameObject content = new("content");
            content.transform.parent = container.transform;

            DataContext context = container.AddComponent<DataContext>();
            TestBinder binder = content.AddComponent<TestBinder>();

            return (context, binder);
        }

        private void DestroyBindingStructure((DataContext context, TestBinder binder) binding)
        {
            GameObject.DestroyImmediate(binding.binder.gameObject);
            GameObject.DestroyImmediate(binding.context.gameObject);
        }
    }
}
