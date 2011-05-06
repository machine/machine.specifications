class Configatron
  # This class can be used to give special powers to a Configatron setting.
  # See Configatron::Delayed and Configatron::Dynamic as examples of how this
  # works.
  # 
  # This class can be subclassed easily. The key is to override the <tt>finalize?</tt>
  # method.
  # 
  # Example:
  #   class RunThreeTimes < Configatron::Proc
  #     def finalize?
  #       self.execution_count == 3
  #     end
  #   end
  # 
  #   configatron.some.rand.generator = RunThreeTimes.new do
  #     rand
  #   end
  #   
  #   configatron.some.rand.generator # => 0.169280668547299
  #   configatron.some.rand.generator # => 0.298880544243205
  #   configatron.some.rand.generator # => 0.421091617110779
  #   configatron.some.rand.generator # => 0.421091617110779
  #   configatron.some.rand.generator # => 0.421091617110779
  class Proc

    # The number of times this Proc has been executed
    attr_accessor :execution_count
    # The block that you want executed when you call the <tt>execute</tt> method.
    attr_accessor :block
    
    # Requires a block to be passed into it.
    def initialize(&block)
      self.execution_count = 0
      self.block = block
    end
    
    # Executes the <tt>block</tt> attribute, ticks up the 
    # <tt>execution_count</tt> attribute by one and then
    # returns the value of the executed <tt>block</tt>
    def execute
      val = self.block.call
      self.execution_count += 1
      return val
    end
    
    # Returns <tt>true</tt> if Configatron should cache the
    # results of the <tt>execute</tt> method, thereby never calling
    # it again.
    def finalize?
      self.execution_count == 1
    end
    
  end
  
  # Tells Configatron to always execute the block at runtime.
  # The results will never be cached.
  # 
  # Example:
  #   configatron.letters = 'a-b-c-d'
  #   configatron.my.letters = Configatron::Delayed.new do
  #     "My letters are: #{configatron.letters}"
  #   end
  #   configatron.my.other.letters = Configatron::Dynamic.new do
  #     "My letters are: #{configatron.a.b.c.d}"
  #   end
  #   
  #   configatron.my.letters # => 'My letters are: a-b-c-d'
  #   configatron.my.other.letters # => 'My letters are: a-b-c-d'
  #   
  #   configatron.letters = 'a-b-c-d-e'
  #   
  #   configatron.my.letters # => 'My letters are: a-b-c-d'
  #   configatron.my.other.letters # => 'My letters are: a-b-c-d-e'
  class Dynamic < Configatron::Proc
    def finalize?
      false
    end
  end
  
  # Tells Configatron to delay execution of the block until
  # runtime. Once run the results of the block will be cached,
  # never to be run again.
  # 
  # Example:
  #   configatron.letters = 'a-b-c-d'
  #   configatron.my.letters = Configatron::Delayed.new do
  #     "My letters are: #{configatron.letters}"
  #   end
  #   configatron.my.other.letters = Configatron::Dynamic.new do
  #     "My letters are: #{configatron.a.b.c.d}"
  #   end
  #   
  #   configatron.my.letters # => 'My letters are: a-b-c-d'
  #   configatron.my.other.letters # => 'My letters are: a-b-c-d'
  #   
  #   configatron.letters = 'a-b-c-d-e'
  #   
  #   configatron.my.letters # => 'My letters are: a-b-c-d'
  #   configatron.my.other.letters # => 'My letters are: a-b-c-d-e'
  class Delayed < Configatron::Proc
  end
  
end