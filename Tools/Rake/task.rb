class Rake::Task
  old_execute = self.instance_method(:execute)

  define_method(:execute) do |args|
    TeamCity.block_opened({ :name => name })
    TeamCity.progress_start name

    begin
      puts "\n[#{name}]\n"
      old_execute.bind(self).call(args)
    ensure
      TeamCity.progress_finish name
      TeamCity.block_closed({ :name => name })
    end
  end
end
