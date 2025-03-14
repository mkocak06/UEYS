﻿using Shared.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    // This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
    // Using non-generic integer types for simplicity and to ease caching logic
    public abstract class BaseEntity
    {
        public virtual long Id { get; protected set; }        
    }
}
