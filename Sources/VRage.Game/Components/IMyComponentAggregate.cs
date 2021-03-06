﻿using System.Collections.Generic;
using System.Diagnostics;
using VRage.Collections;

namespace VRage.Components
{
    /// <summary>
    /// When creating a new aggregate component type, derive from this interface so that you can use extension methods
    /// AddComponent and RemoveComponent
    /// </summary>
    public interface IMyComponentAggregate
    {
        void AfterComponentAdd(MyComponentBase component);
        void BeforeComponentRemove(MyComponentBase component);

        // Note: This should never be null
        MyAggregateComponentList ChildList { get; }
        MyComponentContainer ContainerBase { get; }
    }

    public static class MyComponentAggregateExtensions
    {
        public static void AddComponent(this IMyComponentAggregate aggregate, MyComponentBase component)
        {
            aggregate.ChildList.AddComponent(aggregate.ContainerBase, component);
            component.SetContainer(aggregate.ContainerBase);
            aggregate.AfterComponentAdd(component);
        }

        /// <summary>
        /// Adds to list but doesn't change ownership
        /// </summary>
        public static void AttachComponent(this IMyComponentAggregate aggregate, MyComponentBase component)
        {
            aggregate.ChildList.AddComponent(aggregate.ContainerBase, component);         
        }

        public static void RemoveComponent(this IMyComponentAggregate aggregate, MyComponentBase component)
        {
            int index = aggregate.ChildList.GetComponentIndex(component);
            if (index != -1)
            {
                aggregate.BeforeComponentRemove(component);
                component.SetContainer(null);
                aggregate.ChildList.RemoveComponentAt(index);
            }
        }

/// <summary>
        /// Removes from list, but doesn't change ownership
        /// </summary>
        public static void DetachComponent(this IMyComponentAggregate aggregate, MyComponentBase component)
        {
            int index = aggregate.ChildList.GetComponentIndex(component);
            if (index != -1)
            {
                aggregate.ChildList.RemoveComponentAt(index);
            }
        }


        public static void GetComponentsFlattened(this IMyComponentAggregate aggregate, List<MyComponentBase> output)
        {
            foreach (var child in aggregate.ChildList.Reader)
            {
                var childAggregate = child as IMyComponentAggregate;
                if (childAggregate != null)
                {
                    childAggregate.GetComponentsFlattened(output);
                }
                else
                {
                    output.Add(child);
                }
            }
        }
    }

    public sealed class MyAggregateComponentList
    {
        private List<MyComponentBase> m_components = new List<MyComponentBase>();
        public ListReader<MyComponentBase> Reader { get { return new ListReader<MyComponentBase>(m_components); } }

        public void AddComponent(MyComponentContainer container, MyComponentBase component)
        {
            m_components.Add(component);
        }

        public void RemoveComponentAt(int index)
        {
            m_components.RemoveAtFast(index);
        }

        public int GetComponentIndex(MyComponentBase component)
        {
            return m_components.IndexOf(component);
        }
    }
}
