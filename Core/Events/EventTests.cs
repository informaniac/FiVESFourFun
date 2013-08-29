using System;
using NUnit.Framework;
using FIVES;

namespace Events
{
    [TestFixture()]
    public class EventTests
    {
        private static bool eventWasRaised = false;

        public EventTests ()
        {
        }

        [SetUp()]
        public void setUpTests()
        {
            eventWasRaised = false;
        }

        [Test()]
        public void shouldRaiseNewEntityEventWithCorrectGuid()
        {
            Entity newEntity = new Entity ();
            bool wasRaised = false;
            EntityRegistry.Instance.OnEntityAdded += delegate(object sender, EntityAddedOrRemovedEventArgs e) {
                wasRaised = true;
                Assert.AreEqual(newEntity.Guid, e.elementId);
            };
            EntityRegistry.Instance.addEntity (newEntity);
            Assert.IsTrue(wasRaised);
        }

        [Test()]
        public void shouldRaiseEntityRemovedWithCorrectGuid()
        {
            Entity newEntity = new Entity ();
            bool wasRaised = false;
            EntityRegistry.Instance.OnEntityRemoved += delegate(object sender, EntityAddedOrRemovedEventArgs e) {
                wasRaised = true;
                Assert.AreEqual(newEntity.Guid, e.elementId);
           };
            EntityRegistry.Instance.addEntity (newEntity);
            EntityRegistry.Instance.removeEntity (newEntity.Guid);
            Assert.IsTrue (wasRaised);
        }

        [Test()]
        public void shouldRaiseAttributeChanged()
        {
            ComponentLayout layout = new ComponentLayout ();
            layout.attributes ["a"] = typeof(int);
            ComponentRegistry.Instance.defineComponent ("MyComponent", new Guid (), layout);
            dynamic newEntity = new Entity ();

            Component.AttributeChanged attributeChanged = new Component.AttributeChanged (attributeChangedHandler);
            newEntity.MyComponent.OnAttributeChanged += attributeChanged;
            newEntity.MyComponent.a = 42;
            Assert.IsTrue (eventWasRaised);
        }

        public static void attributeChangedHandler(Object sender, AttributeChangedEventArgs e) {
            eventWasRaised = true;
            Assert.AreEqual(e.attributeName, "a");
            Assert.AreEqual(e.value, 42);
        }

        private void attributeInComponentChangedHandler(Object sender, AttributeInComponentEventArgs e) {
            eventWasRaised = true;
            Assert.AreEqual(e.componentName, "MyComponent");
            Assert.AreEqual(e.attributeName, "a");
            Assert.AreEqual(e.newValue, 42);
        }

        
        [Test()]
        public void shouldRaiseAttributeInComponentChanged()
        {
            ComponentLayout layout = new ComponentLayout ();
            layout.attributes ["a"] = typeof(int);
            ComponentRegistry.Instance.defineComponent ("MyComponent", new Guid (), layout);
            dynamic newEntity = new Entity ();

            Entity.AttributeInComponentChanged attributeInComponentChanged = new Entity.AttributeInComponentChanged (attributeInComponentChangedHandler);
            newEntity.OnAttributeInComponentChanged += attributeInComponentChanged;                         
            newEntity .MyComponent.a = 42;
            Assert.IsTrue (eventWasRaised);
        }
    }
}
