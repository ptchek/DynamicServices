namespace DynamicServices
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public class EntityAction : DynamicAction
	{
		public EntityAction(DynamicType type, MethodInfo method) : base(type, method)
		{
		}

		public override Type Type
		{
			get { return typeof (IDynamicRepository<>).MakeGenericType(new[] {_Type.Type}); }
		}

		public override object Invoke(IDynamicActionInvoker invoker, object instance, IDictionary<string, object> parameters)
		{
			var entity = GetEntity(instance, invoker, parameters);
			return base.Invoke(invoker, entity,
			                   parameters.Where(p => p.Key.ToLowerInvariant() != "id").ToDictionary(x => x.Key, x => x.Value));
		}

		private object GetEntity(object instance, IDynamicActionInvoker invoker, IDictionary<string, object> parameters)
		{
			var getCommand = Type.GetMethod("Get");
			return invoker.Invoke(getCommand, instance,
			                      parameters.Where(p => p.Key.ToLowerInvariant() == "id").ToDictionary(x => x.Key,
			                                                                                           x => x.Value));
		}

		public override IList<DynamicParameter> GetParameters()
		{
			var parameters = base.GetParameters();
			parameters.Add(new DynamicParameter {Name = "id", Type = typeof (object)});
			return parameters;
		}
	}

	public class DynamicParameter
	{
		public string Name { get; set; }
		public Type Type { get; set; }
	}
}