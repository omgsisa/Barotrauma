﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Barotrauma.Items.Components;
using Barotrauma.Networking;
using Microsoft.Xna.Framework;

namespace Barotrauma
{
    partial class Item
    {
        public enum EventType
        {
            ComponentState = 0,
            InventoryState = 1,
            Treatment = 2,
            ChangeProperty = 3,
            Combine = 4,
            Status = 5,
            AssignCampaignInteraction = 6,
            ApplyStatusEffect = 7,
            Upgrade = 8,
            ItemStat = 9,
            DroppedStack = 10,
            SetHighlight = 11,
            SwapItem = 12,

            MinValue = 0,
            MaxValue = 12
        }

        public interface IEventData : NetEntityEvent.IData
        {
            public EventType EventType { get; }
        }

        public readonly struct ComponentStateEventData : IEventData
        {
            public EventType EventType => EventType.ComponentState;
            public readonly ItemComponent Component;
            public readonly ItemComponent.IEventData ComponentData;

            public ComponentStateEventData(ItemComponent component, ItemComponent.IEventData componentData)
            {
                Component = component;
                ComponentData = componentData;
            }
        }

        public readonly struct InventoryStateEventData : IEventData
        {
            public EventType EventType => EventType.InventoryState;
            public readonly ItemContainer Component;
            public readonly Range SlotRange;
            
            public InventoryStateEventData(ItemContainer component, Range slotRange)
            {
                Component = component;
                SlotRange = slotRange;
            }
        }

        public readonly struct ChangePropertyEventData : IEventData
        {
            public EventType EventType => EventType.ChangeProperty;
            public readonly SerializableProperty SerializableProperty;
            public readonly ISerializableEntity Entity;

            public ChangePropertyEventData(SerializableProperty serializableProperty, ISerializableEntity entity)
            {
                if (serializableProperty.GetAttribute<Editable>() == null)
                {
                    DebugConsole.ThrowError($"Attempted to create {nameof(ChangePropertyEventData)} for the non-editable property {serializableProperty.Name}.");
                }
                SerializableProperty = serializableProperty;
                Entity = entity;
            }
        }

        public readonly struct SetItemStatEventData : IEventData
        {
            public EventType EventType => EventType.ItemStat;

            public readonly Dictionary<TalentStatIdentifier, float> Stats;

            public SetItemStatEventData(Dictionary<TalentStatIdentifier, float> stats)
            {
                Stats = stats;
            }
        }

        private readonly struct ItemStatusEventData : IEventData
        {
            public EventType EventType => EventType.Status;

            public readonly bool LoadingRound;

            public ItemStatusEventData(bool loadingRound)
            {
                LoadingRound = loadingRound;
            }
        }
        
        private readonly struct AssignCampaignInteractionEventData : IEventData
        {
            public EventType EventType => EventType.AssignCampaignInteraction;

            public readonly ImmutableArray<Client> TargetClients;

            public AssignCampaignInteractionEventData(IEnumerable<Client> targetClients)
            {
                TargetClients = (targetClients ?? Enumerable.Empty<Client>()).ToImmutableArray();
            }
        }

        public readonly struct ApplyStatusEffectEventData : IEventData
        {
            public EventType EventType => EventType.ApplyStatusEffect;
            public readonly ActionType ActionType;
            public readonly ItemComponent TargetItemComponent;
            public readonly Character TargetCharacter;
            public readonly Limb TargetLimb;
            public readonly Entity UseTarget;
            public readonly Vector2? WorldPosition;

            public ApplyStatusEffectEventData(
                ActionType actionType,
                ItemComponent targetItemComponent = null,
                Character targetCharacter = null,
                Limb targetLimb = null,
                Entity useTarget = null,
                Vector2? worldPosition = null)
            {
                ActionType = actionType;
                TargetItemComponent = targetItemComponent;
                TargetCharacter = targetCharacter;
                TargetLimb = targetLimb;
                UseTarget = useTarget;
                WorldPosition = worldPosition;
            }
        }

        private readonly struct UpgradeEventData : IEventData
        {
            public EventType EventType => EventType.Upgrade;
            public readonly Upgrade Upgrade;

            public UpgradeEventData(Upgrade upgrade)
            {
                Upgrade = upgrade;
            }
        }

        private readonly struct SwapItemEventData : IEventData
        {
            public EventType EventType => EventType.SwapItem;
            public readonly ItemPrefab NewItem;
            public readonly ushort NewId;

            public SwapItemEventData(ItemPrefab newItem, ushort newId)
            {
                NewItem = newItem;
                NewId = newId;
            }
        }
    }
}
