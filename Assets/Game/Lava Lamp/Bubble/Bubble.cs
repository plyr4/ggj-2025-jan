using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Bubble
{
    public float _radius;
    public Vector2 _position;
    public AnimationCurve _radiusOverLifetime;
    public Vector2 _goalPosition;
    public float _moveSpeed;
    public float _randomSeed;
    public float _startTime;
    public float _lifespan;
    public bool _immortal;
    public float _baseRadius;
    public bool _replaced;
    public int _currentHealth = 1;
    public int _colorID;
    public bool _reserved;

    public bool _killed;

    public Rigidbody _rigidbody;

    public static Bubble New(Blob blob, Vector2 position, float radius, float baseRadius, float moveSpeed,
        float lifeSpan, bool immortal, bool reserved)
    {
        Bubble bubble = new Bubble();
        bubble._position = position;
        bubble._baseRadius = baseRadius;
        bubble._moveSpeed = moveSpeed;
        bubble._radius = radius;
        bubble._randomSeed = UnityEngine.Random.value;
        bubble._startTime = Time.time;
        bubble._radiusOverLifetime = blob._newBubbleRadiusOverLifetime;
        bubble._lifespan = lifeSpan;
        bubble._immortal = immortal;
        bubble._colorID = UnityEngine.Random.Range(0, 3);
        bubble._reserved = reserved;
        return bubble;
    }

    public float Age()
    {
        return Time.time - _startTime;
    }

    public bool ShouldDie()
    {
        return !_immortal && Age() > _lifespan && (_baseRadius *
                                                   _radiusOverLifetime.Evaluate(0)) < 0.001f;
    }

    public bool ShouldReplace()
    {
        return !_replaced && Age() > _lifespan * 0.8f;
    }

    public float TargetVisualHeight()
    {
        return 0.1f;
    }

    public void Select()
    {
        throw new NotImplementedException();
    }

    public void Deselect()
    {
        throw new NotImplementedException();
    }

    public int CurrentHealth()
    {
        return _currentHealth;
    }

    public int MaxHealth()
    {
        return 1;
    }

    public void Kill()
    {
        _killed = true;
    }

    public class UpdateOpts
    {
        public Blob _blob;
    }

    public static void UpdateBubbles(UpdateOpts opts)
    {
        List<Bubble> bubbles = opts._blob._bubbles;
        for (int i = bubbles.Count - 1; i > 0; i--)
        {
            Bubble bubble = bubbles[i];
            if (bubble == null) continue;
            if (bubble._reserved) continue;

            if (bubble._killed)
            {
                bubble._goalPosition = bubbles[0]._position;
                bubble.FollowGoal(bubble._moveSpeed * 3f);
                bubble.HandleKilledRadiusUpdate(opts._blob._shrinkRate);
                if (bubble._radius <= 0f)
                    opts._blob.RemoveBubble(bubble);
                continue;
            }

            bubble.FollowGoal(bubble._moveSpeed);
            bubble.HandleLifetimeRadiusUpdate();

            if (bubble.ShouldDie())
            {
                opts._blob.RemoveBubble(bubble);
            }
        }
    }

    public void HandleKilledRadiusUpdate(float shrinkRate)
    {
        _radius -= shrinkRate * Time.deltaTime;
        _radius = Mathf.Max(0f, _radius);
    }

    public void FollowGoal(float speed)
    {
        _position = Vector2.MoveTowards(_position, _goalPosition, speed * Time.deltaTime);

        if (Vector2.Distance(_position, _goalPosition) < 0.001f)
        {
            _position = _goalPosition;
        }
    }

    public void HandleLifetimeRadiusUpdate()
    {
        float radius = _baseRadius *
                       _radiusOverLifetime.Evaluate((Age() / _lifespan));

        _radius = radius;
    }

    public static void HandlePlayerHeat(UpdateOpts opts)
    {
        BubbleMono playerBubbleMono = opts._blob._playerBubbleMono;
        Bubble playerBubble = opts._blob._playerBubbleMono._bubble;
        Rigidbody playerRigidbody = opts._blob._playerBubbleMono._rigidbody;

        Vector2 appliedForce = Vector2.zero;
        Vector2 currentVelocity = playerRigidbody.velocity;

        float maxVelocity = opts._blob._playerBubbleBaseMoveSpeed;
        float fallSpeed = -opts._blob._playerBubbleBaseMoveSpeed / 3.0f;
        float fallRecoveryRate = 0.05f;
        float floatThreshold = 0.1f;

        foreach (HeatSource heatSource in opts._blob._heatSources)
        {
            if (heatSource == null || !heatSource.isActiveAndEnabled) continue;

            Vector2 bounds = heatSource.GetNormalizedWidthBounds();

            float verticalHeatForce = opts._blob._playerBubbleBaseMoveSpeed * playerBubbleMono._mass;
            float riseForce = opts._blob._playerBubbleBaseMoveSpeed * playerBubbleMono._mass;
            float fallForce = opts._blob._playerBubbleBaseMoveSpeed * playerBubbleMono._mass * 0.6f;

            switch (heatSource._mode)
            {
                case HeatSource.Mode.Horizontal:
                    bounds.x -= bounds.x * heatSource._collisionPadding;
                    bounds.y += bounds.y * heatSource._collisionPadding;

                    if (playerBubble._position.x - playerBubble._radius > bounds.x &&
                        playerBubble._position.x + playerBubble._radius < bounds.y)
                    {
                        appliedForce.y += riseForce;
                    }
                    else
                    {
                        appliedForce.y -= fallForce;
                    }

                    break;
                case HeatSource.Mode.Vertical_Left:
                    bounds.x -= bounds.x * heatSource._collisionPadding;
                    bounds.y += bounds.y * heatSource._collisionPadding;

                    if (playerBubble._position.y - playerBubble._radius > bounds.x &&
                        playerBubble._position.y + playerBubble._radius < bounds.y)
                    {
                        appliedForce.x += verticalHeatForce;
                        appliedForce.y += riseForce * 0.5f;
                    }

                    break;
                case HeatSource.Mode.Vertical_Right:
                    bounds.x -= bounds.x * heatSource._collisionPadding;
                    bounds.y += bounds.y * heatSource._collisionPadding;

                    if (playerBubble._position.y - playerBubble._radius > bounds.x &&
                        playerBubble._position.y + playerBubble._radius < bounds.y)
                    {
                        appliedForce.x -= verticalHeatForce;
                        appliedForce.y += riseForce * 0.5f;
                    }

                    break;
            }
        }

        if (appliedForce == Vector2.zero)
        {
            Vector2 targetVelocity = new Vector2(
                currentVelocity.x,
                Mathf.Lerp(
                    currentVelocity.y,
                    currentVelocity.y > fallSpeed ? fallSpeed : currentVelocity.y,
                    fallRecoveryRate
                )
            );

            if (Mathf.Abs(currentVelocity.y - fallSpeed) > floatThreshold)
            {
                playerRigidbody.velocity = Vector2.Lerp(currentVelocity, targetVelocity, 0.1f);
            }
        }
        else
        {
            Vector2 forceDirection = appliedForce.normalized;
            Vector2 velocityDirection = currentVelocity.normalized;

            float alignment = Vector2.Dot(forceDirection, velocityDirection);
            float multiplier = alignment < 0 ? Mathf.Lerp(2.0f, 4.0f, -alignment) : 1.5f;

            appliedForce *= multiplier;
            playerRigidbody.AddForce(appliedForce, ForceMode.Acceleration);

            if (playerRigidbody.velocity.magnitude > maxVelocity)
            {
                playerRigidbody.velocity = Vector2.Lerp(
                    playerRigidbody.velocity,
                    playerRigidbody.velocity.normalized * maxVelocity,
                    0.1f
                );
            }
        }
    }

    public static void HandlePlayerCollisions(UpdateOpts opts)
    {
        List<Bubble> bubbles = opts._blob._bubbles;
        Bubble playerBubble = bubbles[0];

        for (int i = bubbles.Count - 1; i >= 1; i--)
        {
            Bubble bubble = bubbles[i];
            if (bubble._killed) continue;
            if (Vector2.Distance(playerBubble._position, bubble._position) <
                (playerBubble._radius * 1.5f + bubble._radius) * opts._blob._distanceCheckFactor)
            {
                playerBubble._radius += bubble._radius * opts._blob._mergeGrowthRate;
                bubble.Kill();
            }
        }
    }
}