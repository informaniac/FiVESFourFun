﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientManagerPlugin;
using FIVES;

namespace SimpleGravityPlugin
{
    public class SimpleGravityPluginInitializer : IPluginInitializer
    {
        public string Name
        {
            get { return "SimpleGravity"; }
        }

        public List<string> PluginDependencies
        {
            get { return new List<string> { "ClientManager"}; }
        }

        public List<string> ComponentDependencies
        {
            get { return new List<string> {"position"}; }
        }

        public void Initialize()
        {
            RegisterComponents();
            RegisterService();
            RegisterToEvents();
        }

        public void Shutdown()
        {
        }

        private void RegisterComponents() {
            ComponentDefinition gravityDefinition = new ComponentDefinition("gravity");
            gravityDefinition.AddAttribute<float>("groundLevel");
            ComponentRegistry.Instance.Register(gravityDefinition);
        }

        private void RegisterService()
        {
            ClientManager.Instance.RegisterClientService("gravity", false, new Dictionary<string, Delegate>
            {
                {"setGroundlevel", (Action<string, float>)SetGroundlevel}
            });
        }

        private void RegisterToEvents()
        {
            World.Instance.AddedEntity += new EventHandler<EntityEventArgs>(HandleEntityAdded);
            foreach (Entity entity in World.Instance)
            {
                entity.ChangedAttribute += new EventHandler<ChangedAttributeEventArgs>(HandleAttributeChanged);
            }
        }

        private void HandleEntityAdded(Object sender, EntityEventArgs e)
        {
            e.Entity["gravity"]["groundLevel"] = e.Entity["position"]["y"]; // Initialise entities without gravity
            e.Entity.ChangedAttribute += new EventHandler<ChangedAttributeEventArgs>(HandleAttributeChanged);
        }

        private void HandleAttributeChanged(Object sender, ChangedAttributeEventArgs e)
        {
            if (e.Component.Name == "position")
            {
                Entity entity = (Entity)sender;
                if (entity["position"]["y"] != entity["gravity"]["groundLevel"])
                    entity["position"]["y"] = (float)entity["gravity"]["groundLevel"];
            }
        }

        {
            var entity = World.Instance.FindEntity(entityGuid);
            entity["gravity"]["groundLevel"] = groundLevel;
        }
    }
}
