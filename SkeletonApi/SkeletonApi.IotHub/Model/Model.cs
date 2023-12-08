using MediatR;
//using SkeletonApi.Application.Features.MainSubjects;
using SkeletonApi.Shared;
using System;
using System.Text.Json.Serialization;

namespace SkeletonApi.IotHub.Model;

internal record ChatMessage(
    string Message,
    Guid SenderId,
    Guid ReceiverId,
    DateTime SendDate
);

//public class SubjectStore { }


internal record IdentifiedChatMessage(
    string Message,
    string SenderName,
    DateTime SendDate
);


public record MqttRawDataEncapsulation(string topics, MqttRawData mqttRawData);
public record MqttRawData
{

    [JsonPropertyName("timestamp")]
    public virtual long timestamps { get; init; }

    [JsonPropertyName("values")]
    public IEnumerable<MqttRawValue> Values { get; init; }

}

public record MqttRawValue
{
    [JsonPropertyName("id")]
    public string Vid { get; init; }
    [JsonPropertyName("v")]
    public virtual object Value { get; init; }

    [JsonPropertyName("q")]
    public bool Quality { get; init; }

    [JsonPropertyName("t")]
    public long Time { get; init; }

}


//public sealed record MainSubjectListDto
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; }
//    public string Vid { get; set; }

//    public IEnumerable<SubjectsListDto> Subjects { get; set; }
//}

//public sealed record SubjectsListDto
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; }
//    public string Vid { get; set; }
//}

//internal class TestContainer
//{
//    public int Number { get; set; }

//    public TestContainer(int number)
//    {
//        Number = number;
//    }
//}