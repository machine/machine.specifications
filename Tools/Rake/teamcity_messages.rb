class TeamCity
  def self.running?
    ENV.include? 'TEAMCITY_PROJECT_NAME'
  end

  def self.method_missing(method, *args, &block)
    return unless running?

    message_name = camel_case method.to_s
    publish message_name, args[0]
  end

  private
  def self.camel_case(string)
    string \
      .split('_') \
      .inject([]) { |buffer, e| buffer.push(buffer.empty? ? e : e.capitalize) } \
      .join
  end

  def self.publish(message_name, args)
    args = [] << message_name << escaped_array_of(args)
    args = args.flatten.reject { |arg| arg.nil? }

    puts "##teamcity[#{args.join(' ')}]"
  end

  def self.escape(string)
    string \
      .gsub(/\|/, "||") \
      .gsub(/'/, "|'") \
      .gsub(/\r/, "|r") \
      .gsub(/\n/, "|n") \
      .gsub(/\u0085/, "|x") \
      .gsub(/\u2028/, "|l") \
      .gsub(/\u2029/, "|p") \
      .gsub(/\[/, "|[") \
      .gsub(/\]/, "|]")
  end

  def self.escaped_array_of(args)
    return [] if args.nil?

    if args.is_a? Hash
      args.map {
        |key, value| "#{key.to_s}='#{escape value.to_s}'"
      }
    else
      "'#{escape args}'"
    end
  end
end
