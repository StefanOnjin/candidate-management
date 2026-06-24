namespace CandidateManagement.Messaging;

public static class ActivityRoutingKeys
{
    public const string CandidateCreated = "candidate.created";
    public const string CandidateUpdated = "candidate.updated";
    public const string CandidateDeleted = "candidate.deleted";
    public const string CandidateSkillRemoved = "candidate.skill.removed";
    public const string SkillCreated = "skill.created";
    public const string SkillUpdated = "skill.updated";
    public const string SkillDeleted = "skill.deleted";
}
