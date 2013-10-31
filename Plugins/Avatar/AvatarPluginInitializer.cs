using System;
using FIVES;
using System.Collections.Generic;
using AuthPlugin;
using Math;
using ClientManagerPlugin;

namespace AvatarPlugin
{
    public class AvatarPluginInitializer : IPluginInitializer
    {
        #region IPluginInitializer implementation

        public string GetName ()
        {
            return "Avatar";
        }

        public List<string> GetDependencies ()
        {
            return new List<string> { "ClientManager", "Auth", "Renderable", "Motion" };
        }

        public void Initialize ()
        {
            ComponentLayout avatarLayout = new ComponentLayout();
            avatarLayout.AddAttribute<string>("userLogin", null);
            ComponentRegistry.Instance.DefineComponent("avatar", pluginGuid, avatarLayout);

            ClientManager.Instance.RegisterClientService("avatar", true, new Dictionary<string, Delegate> {
				{"getAvatarEntityGuid", (Func<string, string>)GetAvatarEntityGuid},
                {"changeAppearance", (Action<string, string, Vector>)ChangeAppearance},
				{"startAvatarMotionInDirection", (Action<string, Vector>)StartAvatarMotionInDirection},
				{"setAvatarForwardBackwardMotion", (Action<string, float>)SetForwardBackwardMotion},
				{"setAvatarLeftRightMotion", (Action<string, float>)SetLeftRightMotion}
            });

            ClientManager.Instance.NotifyWhenAnyClientAuthenticated((Action<Guid>)delegate(Guid sessionKey) {
                Activate(sessionKey);
                ClientManager.Instance.NotifyWhenClientDisconnected(sessionKey, (Action<Guid>)Deactivate);
            });

            foreach (var guid in EntityRegistry.Instance.GetAllGUIDs()) {
                var entity = EntityRegistry.Instance.GetEntity(guid);
                if (entity.HasComponent("avatar"))
                    avatarEntities[(string)entity["avatar"]["userLogin"]] = entity;
            }
        }

        #endregion

        Entity GetAvatarEntityBySessionKey(Guid sessionKey)
        {
            var userLogin = Authentication.Instance.GetLoginName(sessionKey);
            if (!avatarEntities.ContainsKey(userLogin)) {
                Entity newAvatar = new Entity();
                newAvatar["avatar"]["userLogin"] = userLogin;
                newAvatar["meshResource"]["uri"] = defaultAvatarMesh;
                newAvatar["meshResource"]["visible"] = false;
                EntityRegistry.Instance.AddEntity(newAvatar);
                avatarEntities[userLogin] = newAvatar;
            }

            return avatarEntities[userLogin];
        }

		/// <summary>
        /// Kiara service interface function to let a connected client query the guid of the entity that was created for the avatar
        /// </summary>
        /// <param name="sessionKey">Session Key of the connected client</param>
        /// <returns>The Guid of the Entity used as avatar</returns>
        private string GetAvatarEntityGuid(string sessionKey)
        {
            Entity avatarEntity = GetAvatarEntityBySessionKey(Guid.Parse(sessionKey));
            return avatarEntity.Guid.ToString();
        }
		
        /// <summary>
        /// Activates the avatar entity. Can also be used to update the mesh when its changed.
        /// </summary>
        /// <param name="sessionKey">Client session key.</param>
        void Activate(Guid sessionKey)
        {
            var avatarEntity = GetAvatarEntityBySessionKey(sessionKey);
            avatarEntity["meshResource"]["visible"] = true;
        }

        /// <summary>
        /// Deactivates the avatar entity.
        /// </summary>
        /// <param name="sessionKey">Client session key.</param>
        void Deactivate(Guid sessionKey)
        {
            var avatarEntity = GetAvatarEntityBySessionKey(sessionKey);
            avatarEntity["meshResource"]["visible"] = false;
        }

        /// <summary>
        /// Changes the appearance of the avatar.
        /// </summary>
        /// <param name="sessionKey">Client session key.</param>
        /// <param name="meshURI">New mesh URI.</param>
        /// <param name="scale">New scale.</param>
        void ChangeAppearance(string sessionKey, string meshURI, Vector scale)
        {
            var avatarEntity = GetAvatarEntityBySessionKey(Guid.Parse(sessionKey));

            avatarEntity["meshResource"]["uri"] = meshURI;

            avatarEntity["scale"]["x"] = scale.x;
            avatarEntity["scale"]["y"] = scale.y;
            avatarEntity["scale"]["z"] = scale.z;
        }

        void StartAvatarMotionInDirection(string sessionKey, Vector velocity)
        {
            var avatarEntity = GetAvatarEntityBySessionKey(Guid.Parse(sessionKey));

            avatarEntity["velocity"]["x"] = (float)velocity.x;
            avatarEntity["velocity"]["y"] = (float)velocity.y;
            avatarEntity["velocity"]["z"] = (float)velocity.z;
        }

        void SetForwardBackwardMotion(string sessionKey, float amount)
        {
            var avatarEntity = GetAvatarEntityBySessionKey(Guid.Parse(sessionKey));
            avatarEntity["velocity"]["x"] = amount;
        }

        void SetLeftRightMotion(string sessionKey, float amount)
        {
            var avatarEntity = GetAvatarEntityBySessionKey(Guid.Parse(sessionKey));
            avatarEntity["velocity"]["z"] = amount;
        }
		
        Dictionary<string, Entity> avatarEntities = new Dictionary<string, Entity>();
        // string defaultAvatarMesh = "resources/models/defaultAvatar/avatar.xml3d";
        string defaultAvatarMesh = "resources/models/firetruck/xml3d/firetruck.xml";
        Guid pluginGuid = new Guid("54b1215e-22cc-44ed-bef4-c92e4fb4edb5");
    }
}
