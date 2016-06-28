using Xunit;
using System.Collections.Generic;
using StatlerWaldorfCorp.TeamService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

[assembly:CollectionBehavior(MaxParallelThreads = 1)]

namespace StatlerWaldorfCorp.TeamService
{
    public class MembersControllerTest
    {	    
        [Fact]
        public async void CreateMemberAddsTeamToList() 
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestController", teamId);
            repository.AddTeam(team);        

            Guid newMemberId = Guid.NewGuid();
            Member newMember = new Member(newMemberId);
            await controller.CreateMember(newMember, teamId);

            team = repository.GetTeam(teamId);
            Assert.True(team.Members.Contains(newMember));
        }        

        [Fact]
        public async void CreateMembertoNonexistantTeamReturnsNotFound() 
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();

            Guid newMemberId = Guid.NewGuid();
            Member newMember = new Member(newMemberId);
            var result = await controller.CreateMember(newMember, teamId);

            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async void GetExistingMemberReturnsMember() 
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);        

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member(memberId);
            newMember.FirstName = "Jim";
            newMember.LastName = "Smith";
            await controller.CreateMember(newMember, teamId);

            Member member = (Member)(await controller.GetMember(teamId, memberId) as ObjectResult).Value;
            Assert.Equal(member.ID, newMember.ID);
        }

        [Fact]
        public async void GetNonExistantTeamReturnsNotFound() 
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            var result = await controller.GetMember(Guid.NewGuid(), Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async void GetNonExistantMemberReturnsNotFound() 
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);        

            var result = await controller.GetMember(teamId, Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async void UpdateMemberOverwrites() 
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);        

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member(memberId);
            newMember.FirstName = "Jim";
            newMember.LastName = "Smith";
            await controller.CreateMember(newMember, teamId);

	        team = repository.GetTeam(teamId);
	    
            Member updatedMember = new Member(memberId);
            updatedMember.FirstName = "Bob";
            updatedMember.LastName = "Jones";            
            await controller.UpdateMember(updatedMember, teamId, memberId);
 
            team = repository.GetTeam(teamId);
            Member testMember = team.Members.Where(m => m.ID == memberId).First();
	    
            Assert.Equal(testMember.FirstName, "Bob");
            Assert.Equal(testMember.LastName, "Jones");            
        }           

        [Fact]
        public async void UpdateMembertoNonexistantMemberReturnsNoMatch() 
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestController", teamId);
            repository.AddTeam(team);        

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member(memberId);
            newMember.FirstName = "Jim";
            await controller.CreateMember(newMember, teamId);

            Guid nonMatchedGuid = Guid.NewGuid();
            Member updatedMember = new Member(nonMatchedGuid);
            updatedMember.FirstName = "Bob";
            var result = await controller.UpdateMember(updatedMember, teamId, nonMatchedGuid);            

            Assert.True(result is NotFoundResult);
        }                   
    }
}