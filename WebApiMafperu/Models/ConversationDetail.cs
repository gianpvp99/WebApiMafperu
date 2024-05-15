using System;
using System.Collections.Generic;

namespace WebApiMafperu.Models
{
    public class ConversationDetail
    {
        public List<Conversation> conversations { get; set; }
    }
    public class Interaction
    {
        public string conversationId { get; set; }
        public string addressFrom { get; set; }
        public string participantName { get; set; }
        public bool sendMessage { get; set; }
    }
    public class Conversation
    {
        public string conversationId { get; set; }
        public DateTime conversationStart { get; set; }
        public DateTime conversationEnd { get; set; }
        public double mediaStatsMinConversationMos { get; set; }
        public double mediaStatsMinConversationRFactor { get; set; }
        public string originatingDirection { get; set; }
        public List<Participant> participants { get; set; }
        public List<string> divisionIds { get; set; }
    }
    public class Participant
    {
        public string participantId { get; set; }
        public string participantName { get; set; }
        public string purpose { get; set; }
        public List<Session> sessions { get; set; }
        public string userId { get; set; }
    }
    public class Session
    {
        public string mediaType { get; set; }
        public string sessionId { get; set; }
        public string addressFrom { get; set; }
        public string ani { get; set; }
        public string direction { get; set; }
        public string dnis { get; set; }
        public string sessionDnis { get; set; }
        public string outboundCampaignId { get; set; }
        public string outboundContactId { get; set; }
        public string outboundContactListId { get; set; }
        public string edgeId { get; set; }
        public string remoteNameDisplayable { get; set; }
        public List<Segment> segments { get; set; }
        public List<Metric> metrics { get; set; }
        public List<MediaEndpointStat> mediaEndpointStats { get; set; }
        public string protocolCallId { get; set; }
        public string provider { get; set; }
        public string dispositionAnalyzer { get; set; }
        public string dispositionName { get; set; }
        public string peerId { get; set; }
        public string remote { get; set; }
        public bool? recording { get; set; }
        public string callbackUserName { get; set; }
        public List<string> callbackNumbers { get; set; }
        public DateTime? callbackScheduledTime { get; set; }
        public string scriptId { get; set; }
        public bool? skipEnabled { get; set; }
        public int? timeoutSeconds { get; set; }
        public Flow flow { get; set; }
    }
    public class Flow
    {
        public string flowId { get; set; }
        public string flowName { get; set; }
        public string flowVersion { get; set; }
        public string flowType { get; set; }
        public bool issuedCallback { get; set; }
        public string startingLanguage { get; set; }
    }
    public class Segment
    {
        public DateTime segmentStart { get; set; }
        public DateTime segmentEnd { get; set; }
        public string queueId { get; set; }
        public string disconnectType { get; set; }
        public string segmentType { get; set; }
        public bool conference { get; set; }
        public string wrapUpCode { get; set; }
        public string errorCode { get; set; }
        public List<int?> sipResponseCodes { get; set; }
        public List<string> requestedRoutingUserIds { get; set; }
    }
    public class Metric
    {
        public string name { get; set; }
        public long value { get; set; }
        public DateTime emitDate { get; set; }
    }
    public class MediaEndpointStat
    {
        public List<string> codecs { get; set; }
        public double minMos { get; set; }
        public double minRFactor { get; set; }
        public int maxLatencyMs { get; set; }
        public int receivedPackets { get; set; }
        public int? discardedPackets { get; set; }
    }
    public class Paging
    {
        public string pageSize { get; set; }
        public int pageNumber { get; set; }
    }
    public class Predicate
    {
        public string type { get; set; }
        public string dimension { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }
    }
    public class Claus
    {
        public string type { get; set; }
        public List<Predicate> predicates { get; set; }
    }
    public class SegmentFilter
    {
        public string type { get; set; }
        public List<Claus> clauses { get; set; }
        public List<Predicate> predicates { get; set; }
    }
    public class BodyConversationDetail
    {
        public string interval { get; set; }
        public string order { get; set; }
        public string orderBy { get; set; }
        public Paging paging { get; set; }
        public List<SegmentFilter> segmentFilters { get; set; }
    }
    public class User
    {
        public string id { get; set; }
        public bool active { get; set; }
        public string userName { get; set; }
        public string displayName { get; set; }
        public string title { get; set; }
    }
}