﻿using System;
using CompanyName.MyMeetings.BuildingBlocks.Domain;
using CompanyName.MyMeetings.Modules.Meetings.Domain.MeetingGroupProposals.Events;
using CompanyName.MyMeetings.Modules.Meetings.Domain.MeetingGroups;
using CompanyName.MyMeetings.Modules.Meetings.Domain.Members;

namespace CompanyName.MyMeetings.Modules.Meetings.Domain.MeetingGroupProposals
{
    public class MeetingGroupProposal : Entity, IAggregateRoot
    {
        public MeetingGroupProposalId Id { get; private set; }

        private string _name;

        private string _description;

        private MeetingGroupLocation _location;

        private DateTime _proposalDate;

        private MemberId _proposalUserId;

        private MeetingGroupProposalStatus _status;

        public MeetingGroup CreateMeetingGroup()
        {
            return MeetingGroup.CreateBasedOnProposal(this.Id, _name, _description, _location, _proposalUserId);
        }

        private MeetingGroupProposal()
        {
            // Only for EF.
        }

        private MeetingGroupProposal(
            string name, 
            string description, 
            MeetingGroupLocation location, 
            MemberId proposalUserId)
        {
            Id = new MeetingGroupProposalId(Guid.NewGuid());
            _name = name;
            _description = description;
            _location = location;
            _proposalUserId = proposalUserId;
            _proposalDate = DateTime.UtcNow;
            _status = MeetingGroupProposalStatus.InVerification;

            this.AddDomainEvent(new MeetingGroupProposedDomainEvent(this.Id, _name, _description, proposalUserId, _proposalDate, _location.City, _location.CountryCode));
        }

        public static MeetingGroupProposal ProposeNew(string name,
            string description,
            MeetingGroupLocation location,
            MemberId proposalUserId)
        {
            return new MeetingGroupProposal(name, description, location, proposalUserId);
        }

        public void Accept()
        {
            _status = MeetingGroupProposalStatus.Accepted;

            this.AddDomainEvent(new MeetingGroupProposalAcceptedDomainEvent(this.Id));
        }
    }
}
