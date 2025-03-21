namespace Machine.Specifications.Framework;

public interface IMSpecMessageBus
{
    ValueTask Discovered();

    ValueTask InProgress();

    ValueTask Passed();

    ValueTask Failed();

    ValueTask Skipped();

    ValueTask Cancelled();

    ValueTask SessionArtifact();

    ValueTask TestArtifact();
}
