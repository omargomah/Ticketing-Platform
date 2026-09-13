using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;

namespace Architecture
{
    public class ArchitectureTests
    {
        private const string ApplicationNamespace = "Application";
        private const string DomainNamespace = "Domain";
        private const string InfrastructureNamespace = "Infrastructure";
        private const string APINamespace = "API";



        [Fact]
        public void Domain_Should_HasNoDependencies()
        {
            // arrange
            Assembly assembly = typeof(Domain.AssemblyReference).Assembly;
            string[] otherProjectsNameSpaces = [ApplicationNamespace, InfrastructureNamespace, APINamespace];
            //act
            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjectsNameSpaces).GetResult();

            //assert
            result.IsSuccessful.Should().BeTrue();

        }
        [Fact]
        public void Application_Should_HasNoDependencyOn_API_Infrastructure()
        {
            //arrange
            Assembly assembly = typeof(Application.AssemblyReference).Assembly;
            string[] otherProjectsNameSpaces = [InfrastructureNamespace,  APINamespace];

            //act
            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjectsNameSpaces).GetResult();
            //assert
            result.IsSuccessful.Should().BeTrue();

        }
        [Fact]
        public void Infrastructure_Should_HasNoDependencyOn_API_Domain()
        {
            //arrange
            Assembly assembly = typeof(Infrastructure.AssemblyReference).Assembly;
            string[] otherProjectsNameSpaces = [DomainNamespace, APINamespace];

            //act
            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjectsNameSpaces).GetResult();
            //assert
            result.IsSuccessful.Should().BeTrue();

        }
        [Fact]
        public void API_Should_HasNoDependencyOn_Domain()
        {
            //arrange
            Assembly assembly = typeof(API.AssemblyReference).Assembly;
            string[] otherProjectsNameSpaces = [DomainNamespace];

            //act
            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjectsNameSpaces).GetResult();
            //assert
            result.IsSuccessful.Should().BeTrue();

        }
        

    }

}
