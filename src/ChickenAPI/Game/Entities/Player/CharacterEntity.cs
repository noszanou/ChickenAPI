﻿using System;
using System.Collections.Generic;
using ChickenAPI.Data.TransferObjects;
using ChickenAPI.ECS.Components;
using ChickenAPI.ECS.Entities;
using ChickenAPI.Enums.Game.Entity;
using ChickenAPI.Game.Components;
using ChickenAPI.Game.Maps;
using ChickenAPI.Game.Network;
using ChickenAPI.Packets;
using ChickenAPI.Packets.ServerPackets;

namespace ChickenAPI.Game.Entities.Player
{
    public class CharacterEntity : IPlayerEntity
    {
        private readonly Dictionary<Type, IComponent> _components;

        public CharacterEntity(ISession session)
        {
            Session = session;
            _components = new Dictionary<Type, IComponent>
            {
                { typeof(VisibilityComponent), new VisibilityComponent(this) },
                { typeof(MovableComponent), new MovableComponent(this) },
                {typeof(BattleComponent), new BattleComponent(this) },
                {typeof(CharacterComponent), new CharacterComponent(this) }
            };
        }

        public ISession Session { get; }

        public long Id { get; set; }

        public IEntityManager EntityManager { get; set; }

        public EntityType Type => EntityType.Player;

        public void AddComponent<T>(T component) where T : IComponent
        {
            _components.Add(typeof(T), component);
        }

        public void RemoveComponent<T>(T component) where T : IComponent
        {
            _components.Remove(typeof(T));
        }

        public bool HasComponent<T>() where T : IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        public void TransferEntity(IEntityManager manager)
        {
            if (EntityManager == null)
            {
                EntityManager = manager;
            }
            else
            {
                EntityManager.TransferEntity(this, manager);
            }

            if (manager is IMapLayer map)
            {
                SendPacket(new CInfoPacketBase());
                SendPacket(new CModePacketBase());
                SendPacket(new AtPacketBase());
                SendPacket(new CondPacketBase());
                SendPacket(new CMapPacketBase(map.Map));
                SendPacket(new InPacketBase(this));
            }
        }

        public T GetComponent<T>() where T : class, IComponent => !_components.TryGetValue(typeof(T), out IComponent component) ? null : component as T;

        public void SendPacket(IPacket packetBase) => Session.SendPacket(packetBase);

        public void SendPackets(IEnumerable<IPacket> packets) => Session.SendPackets(packets);

        public void Dispose()
        {
            // TODO Implement a real dispose pattern
            GC.SuppressFinalize(this);
        }
    }
}