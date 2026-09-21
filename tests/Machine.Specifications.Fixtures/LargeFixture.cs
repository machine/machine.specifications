using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Example.Large
{
    public class when_there_are_many_contexts
    {
        public static bool Created = false;

        public when_there_are_many_contexts()
        {
            Created = true;
        }

        It spec = () => { };
    }

    public static class OtherTests
    {
        public static bool Created = false;
    }

    class when_there_are_many_contexts_1
    {
        when_there_are_many_contexts_1()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_2
    {
        when_there_are_many_contexts_2()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_3
    {
        when_there_are_many_contexts_3()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_4
    {
        when_there_are_many_contexts_4()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_5
    {
        when_there_are_many_contexts_5()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_6
    {
        when_there_are_many_contexts_6()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_7
    {
        when_there_are_many_contexts_7()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_8
    {
        when_there_are_many_contexts_8()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_9
    {
        when_there_are_many_contexts_9()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_10
    {
        when_there_are_many_contexts_10()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_11
    {
        when_there_are_many_contexts_11()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_12
    {
        when_there_are_many_contexts_12()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_13
    {
        when_there_are_many_contexts_13()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_14
    {
        when_there_are_many_contexts_14()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_15
    {
        when_there_are_many_contexts_15()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_16
    {
        when_there_are_many_contexts_16()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_17
    {
        when_there_are_many_contexts_17()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_18
    {
        when_there_are_many_contexts_18()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_19
    {
        when_there_are_many_contexts_19()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_20
    {
        when_there_are_many_contexts_20()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_21
    {
        when_there_are_many_contexts_21()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_22
    {
        when_there_are_many_contexts_22()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_23
    {
        when_there_are_many_contexts_23()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_24
    {
        when_there_are_many_contexts_24()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_25
    {
        when_there_are_many_contexts_25()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_26
    {
        when_there_are_many_contexts_26()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_27
    {
        when_there_are_many_contexts_27()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_28
    {
        when_there_are_many_contexts_28()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_29
    {
        when_there_are_many_contexts_29()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_30
    {
        when_there_are_many_contexts_30()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_31
    {
        when_there_are_many_contexts_31()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_32
    {
        when_there_are_many_contexts_32()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_33
    {
        when_there_are_many_contexts_33()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_34
    {
        when_there_are_many_contexts_34()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_35
    {
        when_there_are_many_contexts_35()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_36
    {
        when_there_are_many_contexts_36()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_37
    {
        when_there_are_many_contexts_37()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_38
    {
        when_there_are_many_contexts_38()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_39
    {
        when_there_are_many_contexts_39()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_40
    {
        when_there_are_many_contexts_40()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_41
    {
        when_there_are_many_contexts_41()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_42
    {
        when_there_are_many_contexts_42()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_43
    {
        when_there_are_many_contexts_43()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_44
    {
        when_there_are_many_contexts_44()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_45
    {
        when_there_are_many_contexts_45()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_46
    {
        when_there_are_many_contexts_46()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_47
    {
        when_there_are_many_contexts_47()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_48
    {
        when_there_are_many_contexts_48()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_49
    {
        when_there_are_many_contexts_49()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_50
    {
        when_there_are_many_contexts_50()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_51
    {
        when_there_are_many_contexts_51()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_52
    {
        when_there_are_many_contexts_52()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_53
    {
        when_there_are_many_contexts_53()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_54
    {
        when_there_are_many_contexts_54()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_55
    {
        when_there_are_many_contexts_55()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_56
    {
        when_there_are_many_contexts_56()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_57
    {
        when_there_are_many_contexts_57()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_58
    {
        when_there_are_many_contexts_58()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_59
    {
        when_there_are_many_contexts_59()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_60
    {
        when_there_are_many_contexts_60()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_61
    {
        when_there_are_many_contexts_61()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_62
    {
        when_there_are_many_contexts_62()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_63
    {
        when_there_are_many_contexts_63()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_64
    {
        when_there_are_many_contexts_64()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_65
    {
        when_there_are_many_contexts_65()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_66
    {
        when_there_are_many_contexts_66()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_67
    {
        when_there_are_many_contexts_67()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_68
    {
        when_there_are_many_contexts_68()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_69
    {
        when_there_are_many_contexts_69()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_70
    {
        when_there_are_many_contexts_70()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_71
    {
        when_there_are_many_contexts_71()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_72
    {
        when_there_are_many_contexts_72()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_73
    {
        when_there_are_many_contexts_73()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_74
    {
        when_there_are_many_contexts_74()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_75
    {
        when_there_are_many_contexts_75()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_76
    {
        when_there_are_many_contexts_76()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_77
    {
        when_there_are_many_contexts_77()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_78
    {
        when_there_are_many_contexts_78()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_79
    {
        when_there_are_many_contexts_79()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_80
    {
        when_there_are_many_contexts_80()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_81
    {
        when_there_are_many_contexts_81()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_82
    {
        when_there_are_many_contexts_82()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_83
    {
        when_there_are_many_contexts_83()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_84
    {
        when_there_are_many_contexts_84()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_85
    {
        when_there_are_many_contexts_85()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_86
    {
        when_there_are_many_contexts_86()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_87
    {
        when_there_are_many_contexts_87()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_88
    {
        when_there_are_many_contexts_88()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_89
    {
        when_there_are_many_contexts_89()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_90
    {
        when_there_are_many_contexts_90()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_91
    {
        when_there_are_many_contexts_91()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_92
    {
        when_there_are_many_contexts_92()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_93
    {
        when_there_are_many_contexts_93()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_94
    {
        when_there_are_many_contexts_94()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_95
    {
        when_there_are_many_contexts_95()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_96
    {
        when_there_are_many_contexts_96()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_97
    {
        when_there_are_many_contexts_97()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_98
    {
        when_there_are_many_contexts_98()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_99
    {
        when_there_are_many_contexts_99()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }

    class when_there_are_many_contexts_100
    {
        when_there_are_many_contexts_100()
        {
            OtherTests.Created = true;
        }

        It spec = () => { };
    }
}
