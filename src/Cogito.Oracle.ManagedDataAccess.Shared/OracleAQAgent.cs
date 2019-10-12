namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// The <see cref="OracleAQAgent"/> class represents agents that may be senders or recipients of a message.
    /// </summary>
    public class OracleAQAgent
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public OracleAQAgent()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public OracleAQAgent(string name) :
            this()
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public OracleAQAgent(string name, string address) :
            this(name)
        {
            Address = address;
        }

        /// <summary>
        /// This instance property specifies the address of the agent.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// This instance property specifies the name of the agent.
        /// </summary>
        public string Name { get; }

    }

}
