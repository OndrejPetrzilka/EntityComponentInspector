﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Properties;

public interface IComponentEditor<T> : IComponentEditor, IVisitAdapter<T>
{
}
