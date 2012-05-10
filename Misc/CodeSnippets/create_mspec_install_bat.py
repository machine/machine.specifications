import os
from jinja2 import Environment, DictLoader

#TODO: Add command line handling
vs_version = "2010"
setup_file_name= 'install_vs' + vs_version + '_snippets.bat'
source_folder = 'snippets'
install_folder = '%USERPROFILE%\Documents\Visual Studio '+vs_version+'\Code Snippets\Visual C#\My Code Snippets'
context = locals()

templates = { setup_file_name : 
             '''
mkdir "{{install_folder}}" 2> NUL
{% for file in os.listdir(source_folder) %}
copy "{{os.path.join(source_folder,file)}}" "{{install_folder}}" 
{%- endfor %}
pause
'''}

env = Environment(loader=DictLoader(templates))
template = env.get_template(setup_file_name)
content = template.render(context)
setup_file = open( setup_file_name, 'w')       
setup_file.write(content)
setup_file.close()