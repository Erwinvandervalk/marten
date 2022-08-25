﻿#nullable enable
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Marten.Exceptions;
using Newtonsoft.Json.Linq;

namespace Marten.Services.Json.Transformations.JsonNet
{
    public abstract class EventUpcaster<TEvent>: Json.Transformations.EventUpcaster<TEvent>
        where TEvent : notnull
    {
        public override object FromDbDataReader(ISerializer serializer, DbDataReader dbDataReader, int index) =>
            JsonTransformations.FromDbDataReader(Upcast)(serializer, dbDataReader, index);

        protected abstract TEvent Upcast(JObject oldEvent);
    }

    public abstract class AsyncOnlyEventUpcaster<TEvent>: Json.Transformations.EventUpcaster<TEvent>
        where TEvent : notnull
    {
        public override object FromDbDataReader(ISerializer serializer, DbDataReader dbDataReader, int index) =>
            throw new MartenException(
                $"Cannot use AsyncOnlyEventUpcaster of type {GetType().AssemblyQualifiedName} in the synchronous API.");

        public override async ValueTask<object> FromDbDataReaderAsync(
            ISerializer serializer, DbDataReader dbDataReader, int index, CancellationToken ct
        ) =>
            await JsonTransformations.FromDbDataReaderAsync(UpcastAsync)(serializer, dbDataReader, index, ct)
                .ConfigureAwait(false);

        protected abstract Task<TEvent> UpcastAsync(JObject oldEvent, CancellationToken ct);
    }
}