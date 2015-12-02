using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
	public delegate Task AsyncEventHandler<T>(object sender, T eventArgs);

    public class AsyncEvent<T>
		where T : EventArgs
	{
		private List<AsyncEventHandler<T>> _handlers;
		private Dictionary<EventHandler<T>, AsyncEventHandler<T>> _mapping;

		public bool Any { get; private set; }
		public string Name { get; }

		public AsyncEvent(string name)
		{
			Name = name;
			_handlers = new List<AsyncEventHandler<T>>();
			_mapping = new Dictionary<EventHandler<T>, AsyncEventHandler<T>>();
        }

		public void Add(AsyncEventHandler<T> handler)
		{
			lock (_handlers)
			{
				_handlers.Add(handler);
				Any = true;
			}
		}
		public void Add(EventHandler<T> handler)
		{
			lock (_handlers)
			{
				AsyncEventHandler<T> func = (s, e) => 
				{
					handler(s, e);
					return TaskHelper.CompletedTask;
				};
				_mapping[handler] = func;
				_handlers.Add(func);
				Any = true;
			}
		}

		public void Remove(AsyncEventHandler<T> handler)
		{
			lock (_handlers)
			{
				_handlers.Remove(handler);
				Any = _handlers.Count != 0;
			}
		}
		public void Remove(EventHandler<T> handler)
		{
			lock (_handlers)
			{
				AsyncEventHandler<T> func;
				if (_mapping.TryGetValue(handler, out func))
				{
					_handlers.Remove(func);
					_mapping.Remove(handler);
				}
				Any = _handlers.Count != 0;
			}
		}
		public void Clear()
		{
			lock (_handlers)
			{
				_handlers.Clear();
				_mapping.Clear();
				Any = false;
			}
		}

		public Task Invoke(object sender, T args)
		{
			return Task.WhenAll(_handlers.Select(x => x(sender, args)).ToArray());
		}
	}
}
