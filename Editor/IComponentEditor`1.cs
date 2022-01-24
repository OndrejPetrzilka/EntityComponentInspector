using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Properties;
using Unity.Properties.Adapters;

public interface IComponentEditor<T> : IComponentEditor, IVisit<T>
{
}
