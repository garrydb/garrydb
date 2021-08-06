using System;
using System.Threading.Tasks;

using FluentAssertions;

using GarryDB.Plugins;
using GarryDB.Specs.Builders.Randomized;
using GarryDB.Specs.Plugins.Builders;

using NUnit.Framework;

namespace GarryDB.Specs.Plugins
{
    public static class PluginSpecs
    {
        [TestFixture]
        public class When_a_message_arrives_for_a_registered_route : AsyncSpecification<Plugin>
        {
            private string route;
            private Guid message;
            private Guid receivedMessage;

            protected override Plugin Given()
            {
                route = new RandomStringBuilder().Build();
                message = Guid.NewGuid();

                return new PluginBuilder().Register(route, (Guid message) => receivedMessage = message).Build();
            }

            protected override Task WhenAsync(Plugin subject)
            {
                return subject.RouteAsync(route, message);
            }

            [Test]
            public void It_should_route_the_message_to_the_handler()
            {
                receivedMessage.Should().Be(message);
            }
        }

        [TestFixture]
        public class When_a_message_arrives_for_an_unkown_handler : Specification<Plugin>
        {
            private string route;
            private Func<Task<object>> routeAsync;

            protected override Plugin Given()
            {
                route = new RandomStringBuilder().Build();

                return new PluginBuilder().Build();
            }

            protected override void When(Plugin subject)
            {
                routeAsync = () => subject.RouteAsync(route, Guid.NewGuid());
            }

            [Test]
            public void It_should_not_throw()
            {
                routeAsync.Should().NotThrowAsync();
            }
        }
    }
}
