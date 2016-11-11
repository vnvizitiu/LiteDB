﻿
using System;
using System.IO;
using LiteDB.Tests.NetCore;

namespace LiteDB.Tests
{
    public class EntityInt
    {
        public int Id { get; set; }
    }

    public class EntityGuid
    {
        public Guid Id { get; set; }
    }

    public class EntityOid
    {
        public ObjectId Id { get; set; }
    }

    public class EntityString
    {
        public string Id { get; set; }
    }

    public class AutoIdTest : TestBase
    {
        public void AutoId_Test()
        {
            string test_name = "AutoId_Test";

            var mapper = new BsonMapper();

            mapper.RegisterAutoId(
                (s) => s == null,
                (c) => "doc-" + c.Count()
            );

            using (var db = new LiteDatabase(new MemoryStream(), mapper))
            {
                var cs_int = db.GetCollection<EntityInt>("int");
                var cs_guid = db.GetCollection<EntityGuid>("guid");
                var cs_oid = db.GetCollection<EntityOid>("oid");
                var cs_str = db.GetCollection<EntityString>("str");

                // int32
                var cint_1 = new EntityInt { };
                var cint_2 = new EntityInt { };
                var cint_5 = new EntityInt { Id = 5 }; // set Id, do not generate (jump 3 and 4)!
                var cint_6 = new EntityInt { Id = 0 }; // for int, 0 is empty

                // guid
                var guid = Guid.NewGuid();

                var cguid_1 = new EntityGuid { Id = guid };
                var cguid_2 = new EntityGuid { };

                // oid
                var oid = ObjectId.NewObjectId();

                var coid_1 = new EntityOid { };
                var coid_2 = new EntityOid { Id = oid };

                // string - there is no AutoId for string
                var cstr_1 = new EntityString { };
                var cstr_2 = new EntityString { Id = "mydoc2" };

                cs_int.Insert(cint_1);
                cs_int.Insert(cint_2);
                cs_int.Insert(cint_5);
                cs_int.Insert(cint_6);

                cs_guid.Insert(cguid_1);
                cs_guid.Insert(cguid_2);

                cs_oid.Insert(coid_1);
                cs_oid.Insert(coid_2);

                cs_str.Insert(cstr_1);

                // test for int
                Helper.AssertIsTrue(test_name, 0, 1 == cint_1.Id);
                Helper.AssertIsTrue(test_name, 1, 2 == cint_2.Id);
                Helper.AssertIsTrue(test_name, 2, 5 == cint_5.Id);
                Helper.AssertIsTrue(test_name, 3, 6 == cint_6.Id);

                // test for guid
                Helper.AssertIsTrue(test_name, 4, guid == cguid_1.Id);
                Helper.AssertIsTrue(test_name, 5, Guid.Empty != cguid_2.Id);
                Helper.AssertIsTrue(test_name, 6, cguid_2.Id != cguid_1.Id);

                // test for oid
                Helper.AssertIsTrue(test_name, 7, ObjectId.Empty != coid_1.Id);
                Helper.AssertIsTrue(test_name, 8, oid == coid_2.Id);

                // test for string
                Helper.AssertIsTrue(test_name, 9, "doc-0" == cstr_1.Id);
                Helper.AssertIsTrue(test_name, 10, "mydoc2" == cstr_2.Id);
            }
        }
    }
}