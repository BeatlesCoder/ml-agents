using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class DogAI : Agent
{

    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private float m_Speed;
    [SerializeField] private Transform m_Target;

    // Start is called before the first frame update
    void Start()
    {

    }


    // 每一轮开始时调用
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        transform.position = new Vector3(1, 0.5f, 1);
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;

        float x = Random.Range(-2, 2);
        float z = Random.Range(-2, -2);
        m_Target.position = new Vector3(x, 1f, z);
    }

    // 收集观察结果
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        // 观察8个值
        sensor.AddObservation(m_Target.position);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(m_Rigidbody.velocity.x);
        sensor.AddObservation(m_Rigidbody.velocity.z);
    }

    // 接收动作是否给予奖励
    public override void OnActionReceived(ActionBuffers actions)
    {
       // base.OnActionReceived(actions);
       //Debug.Log("水平轴：" + actions.ContinuousActions[0]);
       //Debug.Log("垂直轴：" + actions.ContinuousActions[1]);

       Vector3 force = Vector3.zero;
       force.x = actions.ContinuousActions[0];
       force.z = actions.ContinuousActions[1];
       m_Rigidbody.AddForce(force*m_Speed);

       if (transform.position.y < 0)
       {
           EndEpisode();    // 此轮训练结果
       }

       float distance = Vector3.Distance(transform.position, m_Target.position);
       if (distance < 1.2f)
       {
           SetReward(1);    // 给奖励
           EndEpisode();
       }
    }

    // 是否手动操作智能体
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}
