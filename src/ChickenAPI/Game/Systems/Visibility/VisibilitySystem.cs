﻿using System;
using System.Linq;
using System.Linq.Expressions;
using ChickenAPI.ECS.Entities;
using ChickenAPI.ECS.Systems;
using ChickenAPI.Enums.Game.Entity;
using ChickenAPI.Enums.Packets;
using ChickenAPI.Game.Components;
using ChickenAPI.Game.Entities.Player;
using ChickenAPI.Packets.ServerPackets;

namespace ChickenAPI.Game.Systems.Visibility
{
    public class VisibilitySystem : NotifiableSystemBase
    {
        public VisibilitySystem(IEntityManager entityManager) : base(entityManager)
        {
        }

        protected override Expression<Func<IEntity, bool>> Filter =>
            entity => entity.Type == EntityType.Player || entity.Type == EntityType.Monster || entity.Type == EntityType.Mate || entity.Type == EntityType.Npc;

        public override void Execute(IEntity entity, SystemEventArgs e)
        {
            switch (e)
            {
                case VisibilitySetInvisibleEventArgs invisibleEvent:
                    SetInvisible(entity, invisibleEvent);
                    break;

                case VisibilitySetVisibleEventArgs visibleEvent:
                    SetVisible(entity, visibleEvent);
                    break;
            }

            throw new NotImplementedException();
        }

        private void SetVisible(IEntity entity, VisibilitySetVisibleEventArgs args)
        {
            entity.GetComponent<VisibilityComponent>().IsVisible = true;
            if (!args.Broadcast)
            {
                return;
            }

            foreach (IEntity i in entity.EntityManager.Entities.Where(Match))
            {
                var player = i as IPlayerEntity;

                player?.SendPacket(new InfoPacketBase());
            }
        }

        private void SetInvisible(IEntity entity, VisibilitySetInvisibleEventArgs args)
        {
            entity.GetComponent<VisibilityComponent>().IsVisible = false;
            if (!args.Broadcast)
            {
                return;
            }

            foreach (IEntity entityy in entity.EntityManager.Entities.Where(Match))
            {
                var player = entityy as IPlayerEntity;
                OutPacketType outPacketType;
                switch (entity.Type)
                {
                    case EntityType.Player:
                        outPacketType = OutPacketType.Character;
                        break;
                    case EntityType.Mate:
                    case EntityType.Npc:
                        outPacketType = OutPacketType.MateOrNpc;
                        break;
                    case EntityType.Monster:
                        outPacketType = OutPacketType.Monster;
                        break;
                    default:
                        // unhandled
                        continue;
                }

                player?.SendPacket(new OutPacketBase { EntityId = entity.Id, Type = outPacketType });
            }
        }
    }
}